USE admins;

CREATE TABLE IF NOT EXISTS users (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id CHAR(36) NOT NULL,
    name VARCHAR(150) NOT NULL,
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    status ENUM('INACTIVE', 'ACTIVE') NOT NULL DEFAULT 'ACTIVE',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    UNIQUE KEY uq_admin_users_external_id (external_id),
    UNIQUE KEY uq_admin_users_email (email)
)
ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS refresh_tokens (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    user_id INT UNSIGNED NOT NULL,
    token_hash VARCHAR(255) NOT NULL,
    expires_at DATETIME NOT NULL,
    revoked_at DATETIME NULL,
    replaced_at DATETIME NULL,
    created_at DATETIME NOT NULL,
    device VARCHAR(255) NULL,
    ip_address VARCHAR(45) NULL,

    PRIMARY KEY (id),
    UNIQUE KEY uq_admin_refresh_tokens_token_hash (token_hash),
    KEY ix_admin_refresh_tokens_user_id (user_id),

    CONSTRAINT fk_admin_refresh_tokens_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE
)
ENGINE=InnoDB;