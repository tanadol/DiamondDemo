-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Mar 30, 2025 at 05:41 PM
-- Server version: 10.4.34-MariaDB
-- PHP Version: 8.2.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `akitacod_testdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `gameplayvalue`
--

CREATE TABLE `gameplayvalue` (
  `userid` int(11) NOT NULL COMMENT 'userid ref',
  `diamond_count` int(11) NOT NULL COMMENT 'no. of current diamond'
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='keep login history of user';

--
-- Dumping data for table `gameplayvalue`
--

INSERT INTO `gameplayvalue` (`userid`, `diamond_count`) VALUES
(1, 17),
(3, 100);

-- --------------------------------------------------------

--
-- Table structure for table `LoginHistory`
--

CREATE TABLE `LoginHistory` (
  `loginid` int(11) NOT NULL,
  `userid` int(11) NOT NULL COMMENT 'userid primary key ref ',
  `logintime` datetime NOT NULL COMMENT 'login datetime'
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='keep login history of user';

--
-- Dumping data for table `LoginHistory`
--

INSERT INTO `LoginHistory` (`loginid`, `userid`, `logintime`) VALUES
(1, 1, '2025-03-30 13:48:38'),
(2, 1, '2025-03-30 15:57:13'),
(3, 1, '2025-03-30 15:57:21'),
(4, 1, '2025-03-30 15:59:30'),
(5, 1, '2025-03-30 15:59:44'),
(6, 1, '2025-03-30 16:00:01'),
(7, 1, '2025-03-30 16:04:26'),
(8, 1, '2025-03-30 16:08:59'),
(9, 1, '2025-03-30 16:11:53'),
(10, 1, '2025-03-30 16:11:59'),
(11, 1, '2025-03-30 16:14:03'),
(12, 1, '2025-03-30 16:18:05'),
(13, 3, '2025-03-30 16:18:25'),
(14, 3, '2025-03-30 16:18:39'),
(15, 1, '2025-03-30 16:20:01'),
(16, 1, '2025-03-30 16:20:14'),
(17, 1, '2025-03-30 16:21:20'),
(18, 1, '2025-03-30 16:22:14'),
(19, 1, '2025-03-30 16:25:03'),
(20, 1, '2025-03-30 16:29:31'),
(21, 1, '2025-03-30 16:42:30'),
(22, 1, '2025-03-30 16:42:39'),
(23, 1, '2025-03-30 16:54:02'),
(24, 1, '2025-03-30 16:55:38'),
(25, 1, '2025-03-30 16:57:12'),
(26, 1, '2025-03-30 16:59:02'),
(27, 1, '2025-03-30 16:59:47'),
(28, 1, '2025-03-30 17:21:37'),
(29, 1, '2025-03-30 17:33:57'),
(30, 1, '2025-03-30 17:34:02'),
(31, 1, '2025-03-30 17:34:07');

-- --------------------------------------------------------

--
-- Table structure for table `User`
--

CREATE TABLE `User` (
  `userid` int(11) NOT NULL COMMENT 'primary key',
  `username` text NOT NULL COMMENT 'username for login',
  `password` text NOT NULL COMMENT 'password hash(md5)',
  `email` text NOT NULL COMMENT 'email'
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Main user data';

--
-- Dumping data for table `User`
--

INSERT INTO `User` (`userid`, `username`, `password`, `email`) VALUES
(1, 'testuser', '098f6bcd4621d373cade4e832627b4f6', 'test@mail.com'),
(3, 'testuser3', '098f6bcd4621d373cade4e832627b4f6', 'test3@mail.com');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `gameplayvalue`
--
ALTER TABLE `gameplayvalue`
  ADD KEY `userid` (`userid`);

--
-- Indexes for table `LoginHistory`
--
ALTER TABLE `LoginHistory`
  ADD PRIMARY KEY (`loginid`),
  ADD KEY `userid` (`userid`);

--
-- Indexes for table `User`
--
ALTER TABLE `User`
  ADD PRIMARY KEY (`userid`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `LoginHistory`
--
ALTER TABLE `LoginHistory`
  MODIFY `loginid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;

--
-- AUTO_INCREMENT for table `User`
--
ALTER TABLE `User`
  MODIFY `userid` int(11) NOT NULL AUTO_INCREMENT COMMENT 'primary key', AUTO_INCREMENT=4;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `gameplayvalue`
--
ALTER TABLE `gameplayvalue`
  ADD CONSTRAINT `gameplayvalue_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `User` (`userid`);

--
-- Constraints for table `LoginHistory`
--
ALTER TABLE `LoginHistory`
  ADD CONSTRAINT `LoginHistory_ibfk_1` FOREIGN KEY (`userid`) REFERENCES `User` (`userid`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
