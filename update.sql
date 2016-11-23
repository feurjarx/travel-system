ALTER TABLE `employee` ADD `email` VARCHAR(100) NULL AFTER `salary`, ADD `password` VARCHAR(32) NULL AFTER `email`, ADD `session` VARCHAR(32) NULL AFTER `password`;

-- 30.10.2016
ALTER TABLE `callout_room` ADD `created_at` INT NOT NULL AFTER `duration`;
INSERT INTO `employee` (`id`, `person_id`, `position`, `created_at`, `salary`, `email`, `password`, `session`) VALUES (NULL, '1', 'директор', '1477808819', '1000000', 'test@test.local', 'C4CA4238A0B923820DCC509A6F75849B', NULL);
ALTER TABLE `callout_room` ADD `payment` INT(6) NOT NULL AFTER `created_at`;
ALTER TABLE `excursion_order` ADD `created_at` INT NOT NULL AFTER `bus_place_number`;

ALTER TABLE `hotel_service_order` ADD `created_at` INT NOT NULL AFTER `provision_at`;

ALTER TABLE `travel_system`.`employee` ADD UNIQUE (`email`);

-- 16.11.2016
ALTER TABLE `room` CHANGE `class` `class` VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Эконом';

-- 17.11.2016
ALTER TABLE `room` CHANGE `class` `type` VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Эконом';

-- 19.11.2016
ALTER TABLE `callout_order` CHANGE `employee_id` `employee_id` INT(11) NULL;
ALTER TABLE `callout_order` DROP FOREIGN KEY `callout_order_ibfk_3`;
ALTER TABLE `callout_order` ADD CONSTRAINT `callout_order_ibfk_3` FOREIGN KEY (`employee_id`) REFERENCES `travel_system`.`employee`(`id`) ON DELETE SET NULL ON UPDATE RESTRICT;

-- 21.11.2016
ALTER TABLE `callout` ADD `is_predefined` TINYINT NOT NULL DEFAULT '0' AFTER `phone`;

-- 23.11.2016
ALTER TABLE `hotel` CHANGE `name` `name` VARCHAR(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;
ALTER TABLE `callout` ADD `details` VARCHAR(100) NULL DEFAULT 'Готовый тур' AFTER `is_predefined`;