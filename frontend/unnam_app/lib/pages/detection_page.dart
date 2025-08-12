import 'dart:async';
import 'dart:convert';
import 'package:camera/camera.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:unnam_app/widgets/custom_back_button.dart'; // 여기에 위젯 import

class DetectionPage extends StatefulWidget {
  final String token;
  final String username;

  const DetectionPage({super.key, required this.token, required this.username});

  @override
  State<DetectionPage> createState() => _DetectionPageState();
}

class _DetectionPageState extends State<DetectionPage> {
  CameraController? _controller;
  List<CameraDescription>? _cameras;
  List<dynamic> _recognitions = [];
  bool _isDetecting = false;
  Timer? _timer;
  int _userPoints = 0;
  bool _isInitializing = true;
  bool _cameraError = false;

  @override
  void initState() {
    super.initState();
    _initializeCamera();
    _fetchUserPoints();
  }

  Future<void> _initializeCamera() async {
    try {
      _cameras = await availableCameras();
      if (_cameras == null || _cameras!.isEmpty) {
        throw CameraException('noCamera', '카메라 없음');
      }

      _controller = CameraController(_cameras!.first, ResolutionPreset.high);
      await _controller!.initialize();
      if (!mounted) return;

      _timer = Timer.periodic(const Duration(seconds: 1), (_) {
        if (!_isDetecting) {
          _isDetecting = true;
          _captureAndProcessImage();
        }
      });
    } catch (e, st) {
      debugPrint('Camera init error: $e\n$st');
      _cameraError = true;
    } finally {
      if (mounted) setState(() => _isInitializing = false);
    }
  }

  Future<void> _fetchUserPoints() async {
    try {
      final response = await http.get(
        Uri.parse('http://10.0.2.2:5020/api/points'),
        headers: {'Authorization': 'Bearer ${widget.token}'},
      );

      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        setState(() {
          _userPoints = data['points'];
        });
      } else {
        print('Failed to fetch points: ${response.statusCode}');
      }
    } catch (e) {
      print('Error fetching points: $e');
    }
  }

  Future<void> _addPoints(int amount) async {
    try {
      final response = await http.post(
        Uri.parse('http://10.0.2.2:5020/api/points/add'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ${widget.token}',
        },
        body: jsonEncode(amount),
      );

      if (response.statusCode == 200) {
        _fetchUserPoints(); // Refresh points after adding
      } else {
        print('Failed to add points: ${response.statusCode}');
      }
    } catch (e) {
      print('Error adding points: $e');
    }
  }

  Future<void> _usePoints(int amount) async {
    try {
      final response = await http.post(
        Uri.parse('http://10.0.2.2:5020/api/points/use'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ${widget.token}',
        },
        body: jsonEncode(amount),
      );

      if (response.statusCode == 200) {
        _fetchUserPoints(); // Refresh points after using
      } else {
        print('Failed to use points: ${response.statusCode}');
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to use points: ${response.body}')),
        );
      }
    } catch (e) {
      print('Error using points: $e');
    }
  }

  Future<void> _captureAndProcessImage() async {
    if (!_controller!.value.isInitialized) {
      _isDetecting = false;
      return;
    }
    final image = await _controller!.takePicture();

    // Send image to server
    var request = http.MultipartRequest(
      'POST',
      Uri.parse('http://localhost:5020/api/detection'),
    ); // Use 10.0.2.2 for Android emulator
    request.headers['Authorization'] = 'Bearer ${widget.token}'; // Add token
    request.files.add(
      await http.MultipartFile.fromPath('imageFile', image.path),
    );

    try {
      var response = await request.send();
      if (response.statusCode == 200) {
        var responseData = await response.stream.toBytes();
        var responseString = String.fromCharCodes(responseData);
        setState(() {
          _recognitions = json.decode(responseString);
        });
      } else {
        print('Error: ${response.statusCode}');
      }
    } catch (e) {
      print('Error sending image: $e');
    } finally {
      _isDetecting = false;
    }
  }

  @override
  void dispose() {
    _timer?.cancel();
    _controller?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // 1) 아직 초기화 중
    if (_isInitializing) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    // 2) 초기화 실패(카메라 없음 등)
    if (_cameraError ||
        _controller == null ||
        !_controller!.value.isInitialized) {
      return Scaffold(
        appBar: AppBar(title: const Text('Object Detection')),
        body: Center(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.error_outline, size: 64, color: Colors.grey),
              const SizedBox(height: 16),
              const Text('카메라를 사용할 수 없습니다'),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: () {
                  setState(() {
                    _isInitializing = true;
                    _cameraError = false;
                  });
                  _initializeCamera(); // 재시도
                },
                child: const Text('다시 시도'),
              ),
            ],
          ),
        ),
      );
    }

    // 3) 정상 프리뷰
    return Scaffold(
      appBar: AppBar(
        title: Text(
          'Object Detection - ${widget.username} (Points: $_userPoints)',
        ),
        // 여기에 뒤로가기 위젯 추가
        leading: const CustomBackButton(),
        actions: [
          IconButton(
            icon: const Icon(Icons.add_circle),
            onPressed: () => _addPoints(10),
            tooltip: 'Add 10 Points',
          ),
          IconButton(
            icon: const Icon(Icons.remove_circle),
            onPressed: () => _usePoints(5),
            tooltip: 'Use 5 Points',
          ),
        ],
      ),
      body: Stack(
        children: [CameraPreview(_controller!), ..._renderBoxes(context)],
      ),
    );
  }

  List<Widget> _renderBoxes(BuildContext context) {
    if (_recognitions.isEmpty) {
      return [];
    }
    final size = MediaQuery.of(context).size;
    final camera = _controller!.value;
    final scaleX = size.width / camera.previewSize!.height;
    final scaleY = size.height / camera.previewSize!.width;

    return _recognitions.map((re) {
      return Positioned(
        left: re['box']['x'] * scaleX,
        top: re['box']['y'] * scaleY,
        width: re['box']['w'] * scaleX,
        height: re['box']['h'] * scaleY,
        child: Container(
          decoration: BoxDecoration(
            border: Border.all(color: Colors.red, width: 2.0),
          ),
          child: Text(
            "${re['label']} ${(re['confidence'] * 100).toStringAsFixed(0)}%",
            style: const TextStyle(
              color: Colors.red,
              fontSize: 16.0,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
      );
    }).toList();
  }
}
