import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:unnam_app/core/auth_provider.dart';
import 'package:unnam_app/pages/login_page.dart';

class LogoutButton extends StatelessWidget {
  const LogoutButton({super.key});

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();

    // 로그인 상태가 아니면 버튼 숨기거나 비활성화
    if (!auth.isLoggedIn) return SizedBox.shrink();

    return IconButton(
      icon: const Icon(Icons.logout),
      tooltip: '로그아웃',
      onPressed: () async {
        await auth.logout();
        Navigator.pushAndRemoveUntil(
          context,
          MaterialPageRoute(builder: (_) => const LoginPage()),
          (_) => false,
        );
      },
    );
  }
}
