<?php

header('Content-Type: application/json');

// Database connection
$servername = "localhost";
$username = "akitacod_testdb";
$password = "akitacodetestdb";
$dbname = "akitacod_testdb";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    echo json_encode(["status" => "error", "message" => "Database connection failed"]);
    exit();
}

// Get input data
$input = json_decode(file_get_contents('php://input'), true);

if (!isset($input['username'])) {
    echo json_encode(["status" => "error", "message" => "Invalid input"]);
    exit();
}

// Sanitize input
$username = $conn->real_escape_string($input['username']);

// Query to get user ID by username
$sql = "SELECT userid FROM User WHERE username = '$username'";
$result = $conn->query($sql);

if ($result->num_rows === 1) {
    $user = $result->fetch_assoc();
    echo json_encode(["status" => "success", "userid" => $user['userid']]);
} else {
    echo json_encode(["status" => "error", "message" => "Username not found"]);
}

// Close connection
$conn->close();

?>
