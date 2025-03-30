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

// Query to get all users and their diamond counts
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
";

$result = $conn->query($sql);

if ($result->num_rows > 0) {
    $users = [];
    while ($row = $result->fetch_assoc()) {
        $users[] = $row;
    }
    echo json_encode(["status" => "success", "data" => $users]);
} else {
    echo json_encode(["status" => "error", "message" => "No users found"]);
}

// Close connection
$conn->close();

?>
