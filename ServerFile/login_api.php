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

if (!isset($input['username']) || !isset($input['password'])) {
    echo json_encode(["status" => "error", "message" => "Invalid input"]);
    exit();
}

// Sanitize input
$username = $conn->real_escape_string($input['username']);
$password = md5($conn->real_escape_string($input['password']));

// Query to check user and get diamond count
$sql = "
    SELECT 
        User.userid, 
        User.username, 
        User.email, 
        gameplayvalue.diamond_count
    FROM 
        User 
    LEFT JOIN 
        gameplayvalue 
    ON 
        User.userid = gameplayvalue.userid
    WHERE 
        User.username = '$username' AND User.password = '$password'
";

$result = $conn->query($sql);

if ($result->num_rows === 1) {
    $user = $result->fetch_assoc();

    // Save login record
    $userid = $user['userid'];
    $logintime = date('Y-m-d H:i:s');
    $insertSql = "INSERT INTO LoginHistory (userid, logintime) VALUES ('$userid', '$logintime')";
    $conn->query($insertSql);

    // Return user data including diamond count
    echo json_encode([
        "status" => "success",
        "data" => [
            "userid" => $user['userid'],
            "username" => $user['username'],
            "email" => $user['email'],
            "diamond_count" => $user['diamond_count'] // Added diamond count
        ]
    ]);
} else {
    echo json_encode(["status" => "error", "message" => "Invalid username or password"]);
}

// Close connection
$conn->close();

?>
