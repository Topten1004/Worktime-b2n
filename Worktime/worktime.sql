/*
 Navicat Premium Data Transfer

 Source Server         : MySQL
 Source Server Type    : MySQL
 Source Server Version : 100421 (10.4.21-MariaDB)
 Source Host           : localhost:3306
 Source Schema         : worktime

 Target Server Type    : MySQL
 Target Server Version : 100421 (10.4.21-MariaDB)
 File Encoding         : 65001

 Date: 19/07/2023 19:08:25
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for __efmigrationshistory
-- ----------------------------
DROP TABLE IF EXISTS `__efmigrationshistory`;
CREATE TABLE `__efmigrationshistory`  (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`MigrationId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of __efmigrationshistory
-- ----------------------------
INSERT INTO `__efmigrationshistory` VALUES ('20230621003653_initial', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230621015841_databaUdate', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230622074114_addPointer', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230622075753_addPassage', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230622081414_addLogTime', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230623071245_addCode', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230623081834_latestDatabase', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230629060456_addType', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230629121512_removeType', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230706151715_addUsers', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230708201925_addWebAccess', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230708201937_Updatedatabase', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230708202141_addAccess', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230718120038_addTag', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230718120046_Nfctag', '6.0.1');
INSERT INTO `__efmigrationshistory` VALUES ('20230719121011_addTagTable', '6.0.1');

-- ----------------------------
-- Table structure for employee
-- ----------------------------
DROP TABLE IF EXISTS `employee`;
CREATE TABLE `employee`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SSN` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `FirstName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `LastName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `EntryDate` datetime(6) NOT NULL,
  `WebAccess` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 28 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of employee
-- ----------------------------
INSERT INTO `employee` VALUES (17, '392261 ', 'Gregory ', 'RECHE', '2023-06-15 00:00:00.000000', 1);
INSERT INTO `employee` VALUES (18, '293132 ', 'Sesilia', 'TIIHIVA', '2023-06-15 00:00:00.000000', 0);
INSERT INTO `employee` VALUES (19, '204655', 'Victorine ', 'CILANE', '2023-06-15 00:00:00.000000', 0);
INSERT INTO `employee` VALUES (20, '375617 ', 'Gabriel', 'TALIA', '2023-06-15 00:00:00.000000', 0);
INSERT INTO `employee` VALUES (21, '274856 ', 'Brigitte', 'ILOAI', '2023-06-15 00:00:00.000000', 0);
INSERT INTO `employee` VALUES (27, '111111', 'GREG', 'TESTEMOI', '2023-06-30 00:00:00.000000', 0);

-- ----------------------------
-- Table structure for passage
-- ----------------------------
DROP TABLE IF EXISTS `passage`;
CREATE TABLE `passage`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmployeeId` int NOT NULL,
  `PointerId` int NOT NULL,
  `LogTime` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Passage_EmployeeId`(`EmployeeId` ASC) USING BTREE,
  INDEX `IX_Passage_PointerId`(`PointerId` ASC) USING BTREE,
  CONSTRAINT `FK_Passage_Employee_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `employee` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_Passage_Pointer_PointerId` FOREIGN KEY (`PointerId`) REFERENCES `pointer` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 147 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of passage
-- ----------------------------
INSERT INTO `passage` VALUES (54, 17, 1, '2023-06-28 04:07:31.135866');
INSERT INTO `passage` VALUES (55, 17, 1, '2023-06-28 04:07:33.553036');
INSERT INTO `passage` VALUES (56, 17, 1, '2023-06-28 04:07:38.800914');
INSERT INTO `passage` VALUES (57, 17, 1, '2023-06-28 04:07:40.996283');
INSERT INTO `passage` VALUES (58, 17, 1, '2023-06-28 04:07:56.392824');
INSERT INTO `passage` VALUES (59, 18, 1, '2023-06-28 04:13:23.822515');
INSERT INTO `passage` VALUES (60, 18, 1, '2023-06-28 04:13:25.743892');
INSERT INTO `passage` VALUES (61, 17, 1, '2023-06-28 04:13:27.703957');
INSERT INTO `passage` VALUES (62, 19, 1, '2023-06-28 04:14:46.523907');
INSERT INTO `passage` VALUES (63, 19, 1, '2023-06-28 04:14:48.471174');
INSERT INTO `passage` VALUES (64, 20, 1, '2023-06-28 04:16:31.779236');
INSERT INTO `passage` VALUES (65, 20, 1, '2023-06-28 04:16:33.510735');
INSERT INTO `passage` VALUES (66, 21, 1, '2023-06-28 04:17:36.327216');
INSERT INTO `passage` VALUES (67, 21, 1, '2023-06-28 04:18:00.435821');
INSERT INTO `passage` VALUES (68, 21, 1, '2023-06-28 04:18:01.931959');
INSERT INTO `passage` VALUES (69, 17, 1, '2023-06-28 05:34:21.144085');
INSERT INTO `passage` VALUES (70, 17, 1, '2023-06-28 05:34:23.511165');
INSERT INTO `passage` VALUES (79, 17, 1, '2023-06-28 21:33:36.570746');
INSERT INTO `passage` VALUES (80, 17, 1, '2023-06-28 21:33:39.865177');
INSERT INTO `passage` VALUES (81, 17, 1, '2023-06-28 21:33:58.446754');
INSERT INTO `passage` VALUES (82, 17, 1, '2023-06-28 21:34:00.200542');
INSERT INTO `passage` VALUES (83, 17, 1, '2023-06-28 21:34:10.963870');
INSERT INTO `passage` VALUES (85, 17, 1, '2023-06-28 21:38:46.807831');
INSERT INTO `passage` VALUES (86, 21, 1, '2023-06-28 22:27:08.677455');
INSERT INTO `passage` VALUES (87, 21, 1, '2023-06-28 22:27:13.229292');
INSERT INTO `passage` VALUES (88, 21, 1, '2023-06-28 22:27:16.828378');
INSERT INTO `passage` VALUES (89, 21, 1, '2023-06-28 22:28:10.461797');
INSERT INTO `passage` VALUES (90, 21, 1, '2023-06-28 22:29:08.281623');
INSERT INTO `passage` VALUES (91, 21, 1, '2023-06-28 22:29:10.648764');
INSERT INTO `passage` VALUES (92, 21, 1, '2023-06-28 22:31:14.568720');
INSERT INTO `passage` VALUES (93, 21, 1, '2023-06-28 22:32:55.723798');
INSERT INTO `passage` VALUES (94, 21, 1, '2023-06-28 22:32:58.057707');
INSERT INTO `passage` VALUES (95, 21, 1, '2023-06-28 22:33:30.554486');
INSERT INTO `passage` VALUES (96, 21, 1, '2023-06-28 22:35:24.064242');
INSERT INTO `passage` VALUES (97, 21, 1, '2023-06-28 22:35:24.855240');
INSERT INTO `passage` VALUES (98, 21, 1, '2023-06-28 22:35:25.666814');
INSERT INTO `passage` VALUES (99, 21, 1, '2023-06-28 22:35:50.517046');
INSERT INTO `passage` VALUES (100, 21, 1, '2023-06-28 22:37:32.338672');
INSERT INTO `passage` VALUES (101, 21, 1, '2023-06-28 22:38:15.345730');
INSERT INTO `passage` VALUES (102, 21, 1, '2023-06-28 22:38:19.005627');
INSERT INTO `passage` VALUES (103, 21, 1, '2023-06-28 22:38:48.010682');
INSERT INTO `passage` VALUES (104, 21, 1, '2023-06-28 22:42:36.304294');
INSERT INTO `passage` VALUES (105, 21, 1, '2023-06-28 22:42:39.152508');
INSERT INTO `passage` VALUES (106, 21, 1, '2023-06-28 22:43:04.449269');
INSERT INTO `passage` VALUES (107, 21, 1, '2023-06-28 22:43:07.090337');
INSERT INTO `passage` VALUES (108, 21, 1, '2023-06-28 22:44:18.667744');
INSERT INTO `passage` VALUES (109, 21, 1, '2023-06-29 04:06:18.489979');
INSERT INTO `passage` VALUES (110, 21, 1, '2023-06-29 04:06:19.548702');
INSERT INTO `passage` VALUES (111, 21, 1, '2023-06-29 04:06:20.398524');
INSERT INTO `passage` VALUES (112, 21, 1, '2023-06-29 05:15:29.252837');
INSERT INTO `passage` VALUES (113, 21, 1, '2023-06-29 05:15:32.805526');
INSERT INTO `passage` VALUES (114, 21, 1, '2023-06-29 05:15:50.126137');
INSERT INTO `passage` VALUES (115, 21, 1, '2023-06-29 05:16:09.213876');
INSERT INTO `passage` VALUES (116, 21, 1, '2023-06-29 05:16:17.364516');
INSERT INTO `passage` VALUES (117, 21, 1, '2023-06-29 05:16:35.072005');
INSERT INTO `passage` VALUES (118, 21, 1, '2023-06-29 05:16:50.069980');
INSERT INTO `passage` VALUES (119, 21, 1, '2023-06-29 05:20:53.167780');
INSERT INTO `passage` VALUES (120, 21, 1, '2023-06-29 05:20:57.978840');
INSERT INTO `passage` VALUES (121, 21, 1, '2023-06-29 05:21:21.161762');
INSERT INTO `passage` VALUES (122, 17, 1, '2023-06-29 12:13:12.165188');
INSERT INTO `passage` VALUES (123, 17, 1, '2023-06-29 12:13:17.670263');
INSERT INTO `passage` VALUES (124, 17, 1, '2023-06-29 12:13:27.815917');
INSERT INTO `passage` VALUES (125, 17, 1, '2023-06-29 12:13:31.273510');
INSERT INTO `passage` VALUES (126, 27, 1, '2023-06-29 19:00:07.448206');
INSERT INTO `passage` VALUES (127, 27, 1, '2023-06-29 19:02:50.105555');
INSERT INTO `passage` VALUES (128, 27, 1, '2023-06-29 19:03:46.324424');
INSERT INTO `passage` VALUES (129, 27, 1, '2023-06-29 19:07:08.601656');
INSERT INTO `passage` VALUES (130, 27, 1, '2023-06-29 19:07:09.318246');
INSERT INTO `passage` VALUES (131, 27, 1, '2023-06-29 19:10:07.670429');
INSERT INTO `passage` VALUES (132, 27, 1, '2023-06-30 11:34:21.345596');
INSERT INTO `passage` VALUES (133, 17, 5, '2023-07-06 18:50:09.271123');
INSERT INTO `passage` VALUES (134, 17, 5, '2023-07-06 18:50:30.433901');
INSERT INTO `passage` VALUES (135, 17, 5, '2023-07-06 18:51:22.767692');
INSERT INTO `passage` VALUES (136, 17, 5, '2023-07-06 18:51:25.243510');
INSERT INTO `passage` VALUES (137, 17, 5, '2023-07-06 18:51:28.240224');
INSERT INTO `passage` VALUES (138, 17, 5, '2023-07-06 18:51:32.051934');
INSERT INTO `passage` VALUES (139, 17, 5, '2023-07-06 18:51:35.842157');
INSERT INTO `passage` VALUES (140, 17, 5, '2023-07-06 18:51:39.090880');
INSERT INTO `passage` VALUES (141, 17, 5, '2023-07-06 18:51:41.099236');
INSERT INTO `passage` VALUES (142, 17, 5, '2023-07-06 18:51:43.521982');
INSERT INTO `passage` VALUES (143, 17, 5, '2023-07-06 18:52:00.045333');
INSERT INTO `passage` VALUES (144, 17, 5, '2023-07-06 18:52:09.635123');
INSERT INTO `passage` VALUES (145, 17, 5, '2023-07-06 18:58:37.385481');
INSERT INTO `passage` VALUES (146, 17, 5, '2023-07-06 18:58:40.411720');

-- ----------------------------
-- Table structure for pointer
-- ----------------------------
DROP TABLE IF EXISTS `pointer`;
CREATE TABLE `pointer`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of pointer
-- ----------------------------
INSERT INTO `pointer` VALUES (1, 'Pointeuse B2N', '999999');
INSERT INTO `pointer` VALUES (5, 'Web Employee', '000001');

-- ----------------------------
-- Table structure for tag
-- ----------------------------
DROP TABLE IF EXISTS `tag`;
CREATE TABLE `tag`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmployeeId` int NOT NULL,
  `Tag` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_Tag_EmployeeId`(`EmployeeId` ASC) USING BTREE,
  CONSTRAINT `FK_Tag_Employee_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `employee` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tag
-- ----------------------------
INSERT INTO `tag` VALUES (1, 17, '343545');
INSERT INTO `tag` VALUES (2, 20, '354544');
INSERT INTO `tag` VALUES (3, 17, '123456');
INSERT INTO `tag` VALUES (4, 17, '123554');

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FirstName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `LastName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Login` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '',
  `MDP` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Role` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 17 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES (1, 'Gregory ', 'RECHE', 'chs@chs.nc', 'admin', 'Admin5', 'MANAGER');
INSERT INTO `user` VALUES (3, 'Chris', 'Groe', 'c@gmail.com', 'chris', 'chris', 'EMPLOYEE');
INSERT INTO `user` VALUES (4, 'Sesilia', 'TIIHIVA', 'c@gmail.com', 'employee1', 'employee1', 'EMPLOYEE');
INSERT INTO `user` VALUES (5, 'Victorine ', 'CILANE', 'c@gmail.com', 'employee2', 'employee2', 'EMPLOYEE');
INSERT INTO `user` VALUES (6, 'Gabriel', 'TALIA', 'c@gmail.com', 'employee3', 'employee3', 'EMPLOYEE');
INSERT INTO `user` VALUES (7, 'Brigitte', 'ILOAI', 'c@gmail.com', 'employee4', 'employee4', 'EMPLOYEE');
INSERT INTO `user` VALUES (16, 'David', 'Riley', 'znaidiukandrii28@gmail.com', 'aaaa', 'aaaa', 'USER');

SET FOREIGN_KEY_CHECKS = 1;
