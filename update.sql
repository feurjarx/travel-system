ALTER TABLE `employee` ADD `email` VARCHAR(100) NULL AFTER `salary`, ADD `password` VARCHAR(32) NULL AFTER `email`, ADD `session` VARCHAR(32) NULL AFTER `password`;
