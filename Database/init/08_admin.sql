USE admin;

CREATE TABLE users (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id CHAR(36) NOT NULL,
    name VARCHAR(150) NOT NULL,
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    status ENUM('INACTIVE', 'ACTIVE') NOT NULL DEFAULT 'ACTIVE',
    created_at DATETIME NOT NULL,
    updated_at DATETIME NULL,

    PRIMARY KEY (id),
    UNIQUE KEY uq_admin_users_external_id (external_id),
    UNIQUE KEY uq_admin_users_email (email)
);