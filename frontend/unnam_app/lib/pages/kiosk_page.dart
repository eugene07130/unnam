import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

class KioskPage extends StatefulWidget {
  final String token;
  final String username;

  const KioskPage({super.key, required this.token, required this.username});

  @override
  State<KioskPage> createState() => _KioskPageState();
}

class _KioskPageState extends State<KioskPage> {
  int _userPoints = 0;

  @override
  void initState() {
    super.initState();
    _fetchUserPoints();
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

  Future<void> _orderItem(String itemName, int cost) async {
    if (_userPoints < cost) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Not enough points for $itemName!')),
      );
      return;
    }

    try {
      final response = await http.post(
        Uri.parse('http://localhost:5020/api/points/use'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ${widget.token}',
        },
        body: jsonEncode(cost),
      );

      if (response.statusCode == 200) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Ordered $itemName for $cost points!')),
        );
        _fetchUserPoints(); // Refresh points after order
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to order $itemName: ${response.body}'),
          ),
        );
        print('Failed to use points: ${response.statusCode}');
      }
    } catch (e) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Error ordering $itemName: $e')));
      print('Error using points: $e');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Kiosk - ${widget.username} (Points: $_userPoints)'),
        backgroundColor: const Color(0xFF00704A), // Starbucks Green
        foregroundColor: Colors.white,
      ),
      body: Container(
        color: const Color(0xFFF3F3F3), // Light grey background
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              _buildMenuItem(
                '아메리카노',
                4000,
                'assets/americano.png',
              ), // Placeholder for image
              const SizedBox(height: 20),
              _buildMenuItem(
                '카페라떼',
                4500,
                'assets/latte.png',
              ), // Placeholder for image
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildMenuItem(String name, int cost, String imagePath) {
    return Card(
      elevation: 5,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
      margin: const EdgeInsets.symmetric(horizontal: 20),
      child: InkWell(
        onTap: () => _orderItem(name, cost),
        borderRadius: BorderRadius.circular(15),
        child: Container(
          width: MediaQuery.of(context).size.width * 0.8,
          padding: const EdgeInsets.all(20),
          child: Column(
            children: [
              // Image.asset(imagePath, height: 100, width: 100), // Uncomment if you add assets
              const SizedBox(height: 10),
              Text(
                name,
                style: const TextStyle(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                  color: Color(0xFF00704A), // Starbucks Green
                ),
              ),
              const SizedBox(height: 5),
              Text(
                '$cost Points',
                style: const TextStyle(fontSize: 18, color: Colors.black87),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
