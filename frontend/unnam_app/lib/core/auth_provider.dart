import 'package:flutter/widgets.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:dio/dio.dart';

class AuthProvider extends ChangeNotifier {
  final _storage = const FlutterSecureStorage();
  String? _token;

  String? get token => _token;
  bool get isLoggedIn => _token != null;

  AuthProvider() {
    _loadToken();
  }

  Future<void> _loadToken() async {
    _token = await _storage.read(key: 'jwt_token');
    notifyListeners();
  }

  Future<void> setToken(String token) async {
    _token = token;
    await _storage.write(key: 'jwt_token', value: token);
    notifyListeners();
  }

  Future<void> logout() async {
    // (선택) 백엔드에 로그아웃 처리 API 호출
    try {
      final dio = Dio();
      dio.options.headers['Authorization'] = 'Bearer $_token';
      await dio.post('http://<your-backend>/api/auth/logout');
    } catch (_) {
      /* 무시해도 됨 */
    }

    // 로컬에 저장된 토큰 삭제
    await _storage.delete(key: 'jwt_token');
    _token = null;
    notifyListeners();
  }
}
