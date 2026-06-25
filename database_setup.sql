-- MySQL local database setup for the Cybersecurity Awareness Bot

CREATE DATABASE IF NOT EXISTS cyber_awareness_bot;

USE cyber_awareness_bot;


CREATE TABLE IF NOT EXISTS cyber_tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(150) NOT NULL,
    description TEXT NOT NULL,
    has_reminder BOOLEAN NOT NULL DEFAULT FALSE,
    reminder_text VARCHAR(100) NULL,
    reminder_date DATETIME NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE IF NOT EXISTS activity_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    action_text TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
