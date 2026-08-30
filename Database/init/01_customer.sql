use customer;

CREATE TABLE IF NOT EXISTS users (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id VARCHAR(255) NOT NULL UNIQUE,
    stripe_customer_id VARCHAR(255) NULL,
    name VARCHAR(255) NOT NULL,
    document VARCHAR(30) NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20) NULL, -- E.164 format
    status TINYINT UNSIGNED NOT NULL DEFAULT 1,
    email_confirmed_at DATETIME NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    deleted_at DATETIME NULL,

    PRIMARY KEY (id),
    UNIQUE KEY uq_users_external_id (external_id),
    UNIQUE KEY uq_users_stripe_customer_id (stripe_customer_id),
    UNIQUE KEY uq_users_document (document),
    UNIQUE KEY uq_users_email (email)
);

CREATE TABLE IF NOT EXISTS logins (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    user_id BIGINT UNSIGNED NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    last_login_at DATETIME NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (id),
    KEY ix_logins_user_id (user_id),
    KEY ix_logins_user_active (user_id, active),

    CONSTRAINT fk_logins_users
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS refresh_tokens (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    login_id BIGINT UNSIGNED NOT NULL,
    token_hash VARCHAR(255) NOT NULL,
    expires_at DATETIME NOT NULL,
    revoked_at DATETIME NULL,
    replaced_at DATETIME NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    device VARCHAR(255) NULL,
    ip_address VARCHAR(45) NULL,

    PRIMARY KEY (id),
    UNIQUE KEY uq_refresh_tokens_token_hash (token_hash),
    
    KEY ix_refresh_tokens_login_id (login_id),
    KEY ix_refresh_tokens_expires_at (expires_at),

    CONSTRAINT fk_refresh_tokens_logins
        FOREIGN KEY (login_id)
        REFERENCES logins(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS addresses (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id CHAR(36) NOT NULL,
    user_id BIGINT UNSIGNED NOT NULL,
    name VARCHAR(50) NULL,
    recipient_name VARCHAR(150) NOT NULL,
    postal_code VARCHAR(20) NOT NULL,
    street VARCHAR(200) NOT NULL,
    number VARCHAR(30) NULL,
    complement VARCHAR(150) NULL,
    district VARCHAR(150) NULL,
    city VARCHAR(255) NOT NULL,
    state VARCHAR(255) NOT NULL,
    country VARCHAR(255) NOT NULL,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    PRIMARY KEY (id),

    UNIQUE KEY uq_addresses_external_id (external_id),

    KEY ix_addresses_user_id (user_id),
    KEY ix_addresses_user_active (user_id, active),
    KEY ix_addresses_user_default (user_id, is_default, active),

    CONSTRAINT fk_addresses_users
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE RESTRICT
);

-- CREATE TABLE IF NOT EXISTS wishlists(
--     id BIGINT AUTO_INCREMENT PRIMARY KEY,
--     user_id BIGINT NOT NULL,
--     catalog_id BIGINT NOT NULL,
--     created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
--     updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
--     FOREIGN KEY (user_id) REFERENCES users(id),
--     -- FOREIGN KEY (catalog_id) REFERENCES catalogs(id)
-- );