// lib/widgets/custom_back_button.dart
import 'package:flutter/material.dart';

class CustomBackButton extends StatelessWidget {
  final VoidCallback? onCustomPressed;

  const CustomBackButton({super.key, this.onCustomPressed});

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: const Icon(Icons.arrow_back),
      onPressed: () {
        // 커스텀 동작이 정의되어 있으면 먼저 실행
        if (onCustomPressed != null) {
          onCustomPressed!();
        }
        // 그 후 기본 뒤로가기 동작 실행
        Navigator.pop(context);
      },
    );
  }
}

//
