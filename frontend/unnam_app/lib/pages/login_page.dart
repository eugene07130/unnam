import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:unnam_app/pages/detection_page.dart';
import 'package:unnam_app/pages/kiosk_page.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:unnam_app/widgets/custom_back_button.dart'; // CustomBackButton 임포트
import 'package:unnam_app/widgets/logout_button.dart'; // LogoutButton 임포트

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();

  Future<void> _login() async {
    final username = _usernameController.text;
    final password = _passwordController.text;

    final response = await http.post(
      Uri.parse('http://10.0.2.2:5020/api/auth/login'), // 안드로이드 에뮬레이터용 주소로 변경
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'username': username, 'password': password}),
    );

    if (response.statusCode == 200) {
      final token = jsonDecode(response.body)['token'];
      Map<String, dynamic> decodedToken = JwtDecoder.decode(token);

      // Assuming the role is in the token's payload
      if (decodedToken['role'] == 'shoes') {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (context) =>
                DetectionPage(token: token, username: username),
          ),
        );
      } else if (decodedToken['role'] == 'cafe') {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (context) => KioskPage(token: token, username: username),
          ),
        );
      } else {
        // Handle other roles or no role
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('You do not have permission to access this feature.'),
          ),
        );
      }
    } else {
      // Handle login failure
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Login failed')));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Login'),
        // 테스트를 위해 뒤로가기 버튼 추가 (const 제거)
        leading: CustomBackButton(),
        // 테스트를 위해 로그아웃 버튼 추가 (const 제거)
        actions: [LogoutButton()],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            TextField(
              controller: _usernameController,
              decoration: const InputDecoration(labelText: 'Username'),
            ),
            TextField(
              controller: _passwordController,
              decoration: const InputDecoration(labelText: 'Password'),
              obscureText: true,
            ),
            const SizedBox(height: 20),
            ElevatedButton(onPressed: _login, child: const Text('Login')),
          ],
        ),
      ),
    );
  }
}
