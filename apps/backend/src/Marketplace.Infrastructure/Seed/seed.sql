-- Reference data seed (idempotent). Executed by the `seed` CLI verb.
-- The admin account is seeded separately in C# (password hashing).

-- Top-level categories
INSERT INTO categories (slug, label_vi, label_en, icon, sort_order) VALUES
    ('excavator', 'Máy đào',              'Excavators',         'truck',        0),
    ('forklift',  'Xe nâng',              'Forklifts',          'forklift',     1),
    ('crane',     'Cần cẩu',              'Cranes',             'construction', 2),
    ('concrete',  'Máy trộn & bơm',       'Concrete Equipment', 'cog',          3),
    ('road',      'Máy lu & làm đường',   'Road Equipment',     'mountain',     4),
    ('parts',     'Phụ tùng & linh kiện', 'Parts & Components', 'wrench',       5)
ON CONFLICT (slug) DO NOTHING;

-- Subcategories (parent resolved by parent slug)
INSERT INTO categories (slug, parent_id, label_vi, label_en, sort_order)
SELECT v.slug, p.id, v.label_vi, v.label_en, v.sort_order
FROM (VALUES
    ('crawler',     'excavator', 'Máy đào bánh xích', 'Crawler Excavator', 0),
    ('wheeled',     'excavator', 'Máy đào bánh lốp',  'Wheeled Excavator', 1),
    ('mini',        'excavator', 'Máy đào mini',       'Mini Excavator',    2),
    ('diesel_fl',   'forklift',  'Xe nâng dầu',        'Diesel Forklift',   0),
    ('electric_fl', 'forklift',  'Xe nâng điện',       'Electric Forklift', 1),
    ('reach_fl',    'forklift',  'Xe nâng cao',        'Reach Truck',       2),
    ('tower_cr',    'crane',     'Cần cẩu tháp',       'Tower Crane',       0),
    ('mobile_cr',   'crane',     'Cần cẩu di động',     'Mobile Crane',      1),
    ('crawler_cr',  'crane',     'Cần cẩu bánh xích',   'Crawler Crane',     2),
    ('mixer',       'concrete',  'Máy trộn bê tông',   'Concrete Mixer',    0),
    ('truck_mix',   'concrete',  'Xe bồn bê tông',      'Transit Mixer',     1),
    ('pump',        'concrete',  'Máy bơm bê tông',     'Concrete Pump',     2),
    ('roller',      'road',      'Xe lu',              'Road Roller',       0),
    ('paver',       'road',      'Máy rải nhựa',        'Asphalt Paver',     1),
    ('cutter',      'road',      'Máy cắt đường',       'Road Cutter',       2),
    ('exc_parts',   'parts',     'Phụ tùng máy đào',   'Excavator Parts',   0),
    ('flft_parts',  'parts',     'Phụ tùng xe nâng',    'Forklift Parts',    1),
    ('other_parts', 'parts',     'Phụ tùng khác',       'Other Parts',       2)
) AS v(slug, parent_slug, label_vi, label_en, sort_order)
JOIN categories p ON p.slug = v.parent_slug
ON CONFLICT (slug) DO NOTHING;

-- Boost packages (idempotent by tier; no unique constraint required)
INSERT INTO boost_packages (tier, name, priority_level, duration_days, price_amount)
SELECT v.tier, v.name, v.priority_level, v.duration_days, v.price_amount
FROM (VALUES
    ('BASIC',    'Basic',        1,  7,  200000::bigint),
    ('FEATURED', 'Featured',     5,  14, 500000::bigint),
    ('MAX',      'Max Priority', 10, 30, 1200000::bigint)
) AS v(tier, name, priority_level, duration_days, price_amount)
WHERE NOT EXISTS (SELECT 1 FROM boost_packages b WHERE b.tier = v.tier);
