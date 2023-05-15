USE `ClubbyBook`;

#region Drops

DROP TABLE IF EXISTS `BookRate`;
DROP TABLE IF EXISTS `AuthorRate`;

DROP TABLE IF EXISTS `FeedbackNotification`;
DROP TABLE IF EXISTS `ConversationNotification`;
DROP TABLE IF EXISTS `SystemNotification`;
DROP TABLE IF EXISTS `Notification`;

DROP TABLE IF EXISTS `BookComment`;
DROP TABLE IF EXISTS `AuthorComment`;
DROP TABLE IF EXISTS `Comment`;

DROP TABLE IF EXISTS `UserRole`;
DROP TABLE IF EXISTS `UserBook`;
DROP TABLE IF EXISTS `Profile`;
DROP TABLE IF EXISTS `User`;

DROP TABLE IF EXISTS `Role`;

DROP TABLE IF EXISTS `News`;

DROP TABLE IF EXISTS `BookAuthor`;
DROP TABLE IF EXISTS `BookGenre`;
DROP TABLE IF EXISTS `Book`;
DROP TABLE IF EXISTS `Author`;
DROP TABLE IF EXISTS `Genre`;

DROP TABLE IF EXISTS `City`;
DROP TABLE IF EXISTS `District`;
DROP TABLE IF EXISTS `Country`;

#endregion


#region Cities

CREATE TABLE `Country` (
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL
) ENGINE = INNODB CHARACTER SET utf8;

CREATE TABLE `District` (
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `CountryId` INT NOT NULL,
  `Name` VARCHAR(100) NOT NULL,
  CONSTRAINT `fk_district_country` FOREIGN KEY(`CountryId`) REFERENCES `Country`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

CREATE TABLE `City` (
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `CountryId` INT NOT NULL,
  `DistrictId` INT NOT NULL,
  `Name` VARCHAR(255) NOT NULL,
  `UrlRewrite` VARCHAR(255) DEFAULT '',
  `Default` BIT(1) NOT NULL DEFAULT 0,
  CONSTRAINT `fk_city_country` FOREIGN KEY(`CountryId`) REFERENCES `Country`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_city_district` FOREIGN KEY(`DistrictId`) REFERENCES `District`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

#endregion


#region Users && Roles && Profile

CREATE TABLE `User`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `EMail` VARCHAR(50) NOT NULL,
  `Password` VARCHAR(32) NOT NULL,
  `CreatedDate` DATETIME NOT NULL,
  `LastAccessDate` DATETIME NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT 0,
  CONSTRAINT `uk_user_email` UNIQUE (`EMail`)
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `Profile`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `Name` VARCHAR(50) DEFAULT '',
  `Surname` VARCHAR(50) DEFAULT '',
  `Nickname` VARCHAR(50) DEFAULT '',
  `Birthday` DATE DEFAULT NULL,
  `Gender` TINYINT DEFAULT 0,
  `CityId` INT DEFAULT NULL,
  `ImagePath` VARCHAR(255) DEFAULT '',
  `UrlRewrite` VARCHAR(255) DEFAULT '',
  CONSTRAINT `fk_profile_user` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_profile_city` FOREIGN KEY (`CityId`) REFERENCES `City` (`Id`) ON UPDATE CASCADE ON DELETE SET NULL
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `Role` (
	`Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
	`Name` VARCHAR(100) NOT NULL,
  CONSTRAINT `uk_role_name` UNIQUE (`Name`)
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `UserRole` (
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `RoleId` INT NOT NULL,
  CONSTRAINT `fk_userrole_user` FOREIGN KEY(`UserId`) REFERENCES `User`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE, 
  CONSTRAINT `fk_userrole_role` FOREIGN KEY(`RoleId`) REFERENCES `Role`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE  
) ENGINE = INNODB CHARACTER SET utf8;

#endregion


#region News

CREATE TABLE `News`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(128) NOT NULL,
  `Message` TEXT NOT NULL,
  `UrlRewrite` VARCHAR(255) DEFAULT '',
  `CreatedDate` DATETIME NOT NULL,
  `LastModifiedDate` DATETIME NOT NULL
) ENGINE = INNODB CHARACTER SET utf8;

#endregion


#region Books && Authors

CREATE TABLE `Book`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(512) NOT NULL,
  `OriginalTitle` VARCHAR(512) DEFAULT '',
  `Description` TEXT NOT NULL,
  `CoverPath` VARCHAR(255) DEFAULT '',
  `UrlRewrite` VARCHAR(255) DEFAULT '',
  `Confirmed` BIT(1) NOT NULL DEFAULT b'1',
  `Collection` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedDate` DATETIME NOT NULL,
  `LastModifiedDate` DATETIME NOT NULL
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `Author` (
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `FullName` VARCHAR(255) NOT NULL,
  `BirthdayYear` INT DEFAULT NULL,
  `DeathYear` INT DEFAULT NULL,
  `ShortDescription` TEXT NOT NULL,
  `Biography` TEXT NOT NULL,
  `PhotoPath` VARCHAR(255) DEFAULT '',
  `UrlRewrite` VARCHAR(255) DEFAULT '',
  `CreatedDate` DATETIME NOT NULL,
  `LastModifiedDate` DATETIME NOT NULL,
  `Type` TINYINT NOT NULL DEFAULT 0,
  `Gender` TINYINT NOT NULL DEFAULT 0
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `BookAuthor` (
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `BookId` INT NOT NULL,
  `AuthorId` INT NOT NULL,
  CONSTRAINT `fk_bookauthor_book` FOREIGN KEY(`BookId`) REFERENCES `Book`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE, 
  CONSTRAINT `fk_bookauthor_author` FOREIGN KEY(`AuthorId`) REFERENCES `Author`(`Id`) ON UPDATE CASCADE ON DELETE CASCADE  
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `Genre`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `UrlRewrite` VARCHAR(255) DEFAULT '',
  `ParentId` INT DEFAULT NULL,
  `Order` INT NOT NULL DEFAULT 0,
  CONSTRAINT `fk_genre_genre` FOREIGN KEY (`ParentId`) REFERENCES `Genre` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `BookGenre`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `BookId` INT NOT NULL,
  `GenreId` INT NOT NULL,
  CONSTRAINT `fk_bookgenre_book` FOREIGN KEY (`BookId`) REFERENCES `Book` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_bookgenre_genre` FOREIGN KEY (`GenreId`) REFERENCES `Genre` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;


#endregion


#region Rate

CREATE TABLE `BookRate`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `BookId` INT NOT NULL,
  `Value` INT NOT NULL DEFAULT 0,
  CONSTRAINT `fk_bookrate_user` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_bookrate_book` FOREIGN KEY (`BookId`) REFERENCES `Book` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

CREATE TABLE `AuthorRate`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `AuthorId` INT NOT NULL,
  `Value` INT NOT NULL DEFAULT 0,
  CONSTRAINT `fk_authorrate_user` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_authorrate_book` FOREIGN KEY (`AuthorId`) REFERENCES `Author` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

#endregion


#region Books && Users

CREATE TABLE `UserBook`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `BookId` INT NOT NULL,
  `Status` INT NOT NULL DEFAULT 0,
  `Offer` INT NOT NULL DEFAULT 0,
  `BookType` INT NOT NULL DEFAULT 0,
  `Comment` VARCHAR(4096) DEFAULT '',
  CONSTRAINT `fk_userbook_user` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_userbook_book` FOREIGN KEY (`BookId`) REFERENCES `Book` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

#endregion


#region Notifications

CREATE TABLE `Notification`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `Message` VARCHAR(4096) NOT NULL DEFAULT '',
  `CreatedDate` DATETIME NOT NULL  
) ENGINE = INNODB CHARACTER SET utf8;

CREATE TABLE `ConversationNotification`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `NotificationId` INT NOT NULL,
  `IsNew` BIT(1) NOT NULL DEFAULT 1,
  `Direction` TINYINT NOT NULL DEFAULT 0,
  `FromUserId` INT NOT NULL,
  `ToUserId` INT NOT NULL,
  CONSTRAINT `fk_conversationnotification_notificationid` FOREIGN KEY (`NotificationId`) REFERENCES `Notification` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  -- Users never removed, thats why cascade removing is ok
  CONSTRAINT `fk_conversationnotification_fromuser` FOREIGN KEY (`FromUserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_conversationnotification_touser` FOREIGN KEY (`ToUserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;


CREATE TABLE `SystemNotification`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `NotificationId` INT NOT NULL,
  `Type` TINYINT NOT NULL DEFAULT 0,
  `IsNew` BIT(1) NOT NULL DEFAULT 1,
  `OwnerUserId` INT NOT NULL,
  CONSTRAINT `fk_systemnotification_notificationid` FOREIGN KEY (`NotificationId`) REFERENCES `Notification` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_systemnotification_owneruser` FOREIGN KEY (`OwnerUserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

CREATE TABLE `FeedbackNotification`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `NotificationId` INT NOT NULL,
  `IsNew` BIT(1) NOT NULL DEFAULT 1,
  `OwnerUserId` INT,
  CONSTRAINT `fk_feedbacknotification_notificationid` FOREIGN KEY (`NotificationId`) REFERENCES `Notification` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_feedbacknotification_owneruser` FOREIGN KEY (`OwnerUserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE SET NULL
) ENGINE = INNODB CHARACTER SET utf8;


#endregion


#region Comments

CREATE TABLE `Comment`(
  `Id` INT PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `Message` VARCHAR(4096) NOT NULL DEFAULT '',
  `CreatedDate` DATETIME NOT NULL,
  CONSTRAINT `fk_comment_user` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

CREATE TABLE `BookComment`(
  `CommentId` INT NOT NULL,
  `BookId` INT NOT NULL,
  PRIMARY KEY (`CommentId`, `BookId`),
  CONSTRAINT `fk_bookcomment_comment` FOREIGN KEY (`CommentId`) REFERENCES `Comment` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_bookcomment_book` FOREIGN KEY (`BookId`) REFERENCES `Book` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

CREATE TABLE `AuthorComment`(
  `CommentId` INT NOT NULL,
  `AuthorId` INT NOT NULL,
  PRIMARY KEY (`CommentId`, `AuthorId`),
  CONSTRAINT `fk_authorcomment_comment` FOREIGN KEY (`CommentId`) REFERENCES `Comment` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT `fk_authorcomment_author` FOREIGN KEY (`AuthorId`) REFERENCES `Author` (`Id`) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE = INNODB CHARACTER SET utf8;

#endregion