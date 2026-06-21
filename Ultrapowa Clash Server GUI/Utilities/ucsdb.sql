/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 100119
Source Host           : localhost:3306
Source Database       : ucsdb

Target Server Type    : MYSQL
Target Server Version : 100119
File Encoding         : 65001

Date: 2017-03-16 17:22:16
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for clan
-- ----------------------------
DROP TABLE IF EXISTS `clan`;
CREATE TABLE `clan` (
  `ClanId` bigint(20) NOT NULL,
  `LastUpdateTime` datetime NOT NULL,
  `Data` text CHARACTER SET utf8mb4 NOT NULL,
  PRIMARY KEY (`ClanId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of clan
-- ----------------------------

-- ----------------------------
-- Table structure for player
-- ----------------------------
DROP TABLE IF EXISTS `player`;
CREATE TABLE `player` (
  `PlayerId` bigint(20) NOT NULL,
  `AccountStatus` tinyint(4) NOT NULL,
  `AccountPrivileges` tinyint(4) NOT NULL,
  `LastUpdateTime` datetime NOT NULL,
  `IPAddress` varchar(255) DEFAULT NULL,
  `Avatar` text CHARACTER SET utf8mb4 NOT NULL,
  `GameObjects` text CHARACTER SET utf8mb4 NOT NULL,
  PRIMARY KEY (`PlayerId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of player
-- ----------------------------
SET FOREIGN_KEY_CHECKS=1;
