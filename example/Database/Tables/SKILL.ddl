DROP TABLE IF EXISTS `SKILL`;
CREATE TABLE `SKILL` (
	`skill_id` varchar(50) NOT NULL,
	`skillName` varchar(255) NOT NULL,
	`level` INT NOT NULL,
	`power` INT NOT NULL,
	`account` varchar(255) NOT NULL,
	`classTest` varchar(255) NOT NULL,
	`args` INT NOT NULL,
	PRIMARY KEY (`skill_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
