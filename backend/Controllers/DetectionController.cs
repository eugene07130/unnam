using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetectionController : ControllerBase
{
    private readonly InferenceSession _session;

    public DetectionController(InferenceSession session)
        => _session = session;

    /// <summary>
    /// 이미지 파일을 받아서 객체 검출 결과를 반환
    /// </summary>
    [HttpPost("detect")]
    [Consumes("multipart/form-data")]  // multipart/form-data 로 처리하도록
    [ProducesResponseType(typeof(IEnumerable<DetectionResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Detect([FromForm] DetectionRequest req)
    {
        var imageFile = req.ImageFile;
        if (imageFile == null || imageFile.Length == 0)
            return BadRequest("No image file provided.");

        try
        {
            using var stream = imageFile.OpenReadStream();
            using var image = Image.Load<Rgb24>(stream);

            var (tensor, scale) = PreprocessImage(image);
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("images", tensor)
            };
            using var results = _session.Run(inputs);
            var output = results.FirstOrDefault(x => x.Name == "output0")?.AsTensor<float>();
            if (output == null)
                return StatusCode(500, "Failed to get model output.");


            var raw = PostprocessOutput(output, scale);
            var dtoList = PostprocessOutput(output, scale);
            return Ok(dtoList);

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // DTO 클래스들
    public class DetectionRequest
    {
        [Required]
        [FromForm(Name = "imageFile")]
        public IFormFile ImageFile { get; set; }
    }

    public class DetectionResultDto
    {
        public BoxDto Box { get; set; }
        public string Label { get; set; }
        public float  Confidence { get; set; }
    }

    public class BoxDto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
    }


    private (DenseTensor<float>, float) PreprocessImage(Image<Rgb24> image)
    {
        const int targetWidth = 640;
        const int targetHeight = 640;

        var originalWidth = image.Width;
        var originalHeight = image.Height;

        var scale = Math.Min((float)targetWidth / originalWidth, (float)targetHeight / originalHeight);
        var newWidth = (int)(originalWidth * scale);
        var newHeight = (int)(originalHeight * scale);

        image.Mutate(x => x.Resize(newWidth, newHeight));

        // Create a new image with padding to make it 640x640
        using var paddedImage = new Image<Rgb24>(targetWidth, targetHeight, Color.Black);
        paddedImage.Mutate(x => x.DrawImage(image, new Point((targetWidth - newWidth) / 2, (targetHeight - newHeight) / 2), 1f));

        var tensor = new DenseTensor<float>(new[] { 1, 3, targetHeight, targetWidth });
        var mean = new[] { 0.485f, 0.456f, 0.406f };
        var stddev = new[] { 0.229f, 0.224f, 0.225f };

        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                var pixel = paddedImage[x, y];
                tensor[0, 0, y, x] = ((pixel.R / 255f) - mean[0]) / stddev[0];
                tensor[0, 1, y, x] = ((pixel.G / 255f) - mean[1]) / stddev[1];
                tensor[0, 2, y, x] = ((pixel.B / 255f) - mean[2]) / stddev[2];
            }
        }

        return (tensor, scale);
    }
    
    private List<DetectionResultDto> PostprocessOutput(Tensor<float> output, float scale)
    {
        var detections = new List<DetectionResultDto>();

        for (int i = 0; i < output.Dimensions[2]; i++)
        {
            var confidence = output[0, 4, i];
            if (confidence <= 0.5f) 
                continue;

            var x = output[0, 0, i] / scale;
            var y = output[0, 1, i] / scale;
            var w = output[0, 2, i] / scale;
            var h = output[0, 3, i] / scale;

            // 클래스 인덱스 계산
            int classIndex = 0;
            float maxProb = 0;
            for (int j = 4; j < output.Dimensions[1]; j++)
            {
                var p = output[0, j, i];
                if (p > maxProb)
                {
                    maxProb = p;
                    classIndex = j - 4;
                }
            }

            detections.Add(new DetectionResultDto
            {
                Box = new BoxDto { X = x, Y = y, W = w, H = h },
                Label = classIndex.ToString(),  // 실제 이름 리스트가 있으면 그걸 쓰세요
                Confidence = confidence
            });
        }

        return detections;
    }
}

