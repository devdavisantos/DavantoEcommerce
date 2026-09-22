USE catalogs;

CREATE TABLE IF NOT EXISTS categories (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id CHAR(36) NOT NULL,
    name VARCHAR(150) NOT NULL,
    slug VARCHAR(180) NOT NULL,
    active TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    UNIQUE KEY uq_categories_external_id (external_id),
    UNIQUE KEY uq_categories_slug (slug),

    KEY ix_categories_active (active)
)
ENGINE=InnoDB;


CREATE TABLE IF NOT EXISTS brands (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id CHAR(36) NOT NULL,
    name VARCHAR(150) NOT NULL,
    slug VARCHAR(180) NOT NULL,
    active TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    UNIQUE KEY uq_brands_external_id (external_id),
    UNIQUE KEY uq_brands_slug (slug),

    KEY ix_brands_active (active)
)
ENGINE=InnoDB;


CREATE TABLE IF NOT EXISTS products (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id CHAR(36) NOT NULL,

    category_id INT UNSIGNED NULL,
    brand_id INT UNSIGNED NULL,

    title VARCHAR(255) NOT NULL,
    slug VARCHAR(300) NOT NULL,
    description TEXT NOT NULL,

    sku VARCHAR(100) NULL,

    price DECIMAL(15,2) NOT NULL,
    discount_price DECIMAL(15,2) NULL,

    stock INT UNSIGNED NOT NULL DEFAULT 0,

    `condition` ENUM(
        'NEW',
        'USED',
        'REFURBISHED'
    ) NOT NULL DEFAULT 'NEW',

    status ENUM(
        'DRAFT',
        'ACTIVE',
        'DISCONTINUED',
        'INACTIVE'
    ) NOT NULL DEFAULT 'DRAFT',

    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    UNIQUE KEY uq_products_external_id (external_id),
    UNIQUE KEY uq_products_slug (slug),
    UNIQUE KEY uq_products_sku (sku),

    KEY ix_products_category_id (category_id),
    KEY ix_products_brand_id (brand_id),
    KEY ix_products_status (status),

    CONSTRAINT fk_products_category
        FOREIGN KEY (category_id)
        REFERENCES categories(id),

    CONSTRAINT fk_products_brand
        FOREIGN KEY (brand_id)
        REFERENCES brands(id),

    CONSTRAINT chk_products_price
        CHECK (price >= 0),

    CONSTRAINT chk_products_discount_price
        CHECK (
            discount_price IS NULL
            OR (
                discount_price >= 0
                AND discount_price < price
            )
        )
)
ENGINE=InnoDB;


CREATE TABLE IF NOT EXISTS product_images (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    external_id CHAR(36) NOT NULL,
    product_id BIGINT UNSIGNED NOT NULL,

    image_key VARCHAR(500) NOT NULL,

    is_primary TINYINT(1) NOT NULL DEFAULT 0,
    sort_order INT UNSIGNED NOT NULL DEFAULT 0,

    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (id),

    UNIQUE KEY uq_product_images_external_id (external_id),
    UNIQUE KEY uq_product_images_key (image_key),

    KEY ix_product_images_product_id (product_id),
    KEY ix_product_images_product_order (product_id, sort_order),

    CONSTRAINT fk_product_images_product
        FOREIGN KEY (product_id)
        REFERENCES products(id)
        ON DELETE CASCADE
)
ENGINE=InnoDB;