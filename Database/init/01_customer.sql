use customer;

CREATE TABLE IF NOT EXISTS users (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    external_id VARCHAR(255) NOT NULL UNIQUE, -- nao pode ser null
    first_name VARCHAR(255) NOT NULL, -- não pode ser null
    last_name VARCHAR(255) NOT NULL, -- não pode ser null
    document VARCHAR(30) UNIQUE, -- CPF e documento universal, pode ser null e depois só sera atualizado quando o usuario colocar
    email VARCHAR(255) NOT NULL UNIQUE, -- não pode ser null
    phone VARCHAR(20) NOT NULL, -- E.164 format - pode ser null
    stripe_customer_id VARCHAR(255) UNIQUE, -- pode ser null
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS addresses (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT,
    type VARCHAR(50), -- 'billing' ou 'shipping'
    line1 VARCHAR(255),
    line2 VARCHAR(255),
    city VARCHAR(255),
    state VARCHAR(255),
    postal_code VARCHAR(20),
    country VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);


customer.preferences
customer.wishlists
customer.notifications


Dados do cliente.

stripe_customer_id
created_at
updated_at