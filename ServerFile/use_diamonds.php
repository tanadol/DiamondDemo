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

if (!isset($input['userid']) || !isset($input['diamonds_used'])) {
    echo json_encode(["status" => "error", "message" => "Invalid input"]);
    exit();
}

// Sanitize input
$userid = $conn->real_escape_string($input['userid']);
$diamondsUsed = (int) $conn->real_escape_string($input['diamonds_used']);

// Query to get the current diamond count
$sql = "SELECT diamond_count FROM gameplayvalue WHERE userid = '$userid'";
$result = $conn->query($sql);

if ($result->num_rows === 1) {
    $user = $result->fetch_assoc();
    $currentDiamonds = $user['diamond_count'];

    // Check if the user has enough diamonds
    if ($currentDiamonds >= $diamondsUsed) {
        // Update diamond count
        $newDiamondCount = $currentDiamonds - $diamondsUsed;
        $updateSql = "UPDATE gameplayvalue SET diamond_count = '$newDiamondCount' WHERE userid = '$userid'";

        if ($conn->query($updateSql) === TRUE) {
            echo json_encode(["status" => "success", "message" => "Diamonds used successfully", "new_diamond_count" => $newDiamondCount]);
        } else {
            echo json_encode(["status" => "error", "message" => "Failed to update diamond count"]);
        }
    } else {
        echo json_encode(["status" => "error", "message" => "Not enough diamonds"]);
    }
} else {
    echo json_encode(["status" => "error", "message" => "User not found"]);
}

// Close connection
$conn->close();

?>