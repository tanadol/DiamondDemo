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

if (!isset($input['username']) || !isset($input['password']) || !isset($input['email'])) {
    echo json_encode(["status" => "error", "message" => "Invalid input"]);
    exit();
}

// Sanitize input
$username = $conn->real_escape_string($input['username']);
$password = md5($conn->real_escape_string($input['password']));
$email = $conn->real_escape_string($input['email']);

// Check if username or email already exists
$checkSql = "SELECT * FROM User WHERE username='$username' OR email='$email'";
$checkResult = $conn->query($checkSql);
if ($checkResult->num_rows > 0) {
    echo json_encode(["status" => "error", "message" => "Username or email already exists"]);
    exit();
}

// Insert new user
$sql = "INSERT INTO User (username, password, email) VALUES ('$username', '$password', '$email')";
if ($conn->query($sql) === TRUE) {
    // Get the new user's userid
    $userid = $conn->insert_id;

    // Insert the initial gameplay value
    $diamondCount = 100;
    $gameplaySql = "INSERT INTO gameplayvalue (userid, diamond_count) VALUES ('$userid', '$diamondCount')";
    
    if ($conn->query($gameplaySql) === TRUE) {
        echo json_encode(["status" => "success", "message" => "User registered and gameplay value set"]);
    } else {
        echo json_encode(["status" => "error", "message" => "Failed to set initial gameplay value"]);
    }
} else {
    echo json_encode(["status" => "error", "message" => "Registration failed"]);
}

// Close connection
$conn->close();

?>