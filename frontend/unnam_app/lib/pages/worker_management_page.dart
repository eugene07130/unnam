import 'package:flutter/material.dart';

class WorkerManagementPage extends StatelessWidget {
  const WorkerManagementPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('작업자 관리')),
      body: const Center(child: Text('작업자 목록을 여기에 표시합니다.')),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          // 작업자 추가하는 로직 여기에 작성
        },
        child: const Icon(Icons.add),
      ),
    );
  }
}
