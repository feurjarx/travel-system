ALTER TABLE `employee` ADD `email` VARCHAR(100) NULL AFTER `salary`, ADD `password` VARCHAR(32) NULL AFTER `email`, ADD `session` VARCHAR(32) NULL AFTER `password`;

-- 30.10.2016
ALTER TABLE `callout_room` ADD `created_at` INT NOT NULL AFTER `duration`;
INSERT INTO `employee` (`id`, `person_id`, `position`, `created_at`, `salary`, `email`, `password`, `session`) VALUES (NULL, '1', 'директор', '1477808819', '1000000', 'test@test.local', 'C4CA4238A0B923820DCC509A6F75849B', NULL);
ALTER TABLE `callout_room` ADD `payment` INT(6) NOT NULL AFTER `created_at`;
ALTER TABLE `excursion_order` ADD `created_at` INT NOT NULL AFTER `bus_place_number`;

ALTER TABLE `hotel_service_order` ADD `created_at` INT NOT NULL AFTER `provision_at`;
