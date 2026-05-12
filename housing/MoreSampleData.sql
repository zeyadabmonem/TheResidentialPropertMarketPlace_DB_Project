-- Add More Sample Data to Make the Application Look Rich
-- Run this to add comprehensive test data

-- Add more Representatives
INSERT INTO dbo.representative (repres_name, email, phone, license_no) VALUES
('Khaled Ahmed', 'khaled@prestige.com', '01223456789', 'REP-004'),
('Nadia Mahmoud', 'nadia@prestige.com', '01134567890', 'REP-005'),
('Omar Samir', 'omar.s@prestige.com', '01045678901', 'REP-006'),
('Fatima Youssef', 'fatima@prestige.com', '01256789012', 'REP-007'),
('Ali Hassan', 'ali.h@prestige.com', '01167890123', 'REP-008');

-- Add more Clients
INSERT INTO dbo.client (name, phone, email, national_id) VALUES
('Mona Ahmed', '01234567892', 'mona.a@email.com', '29501021234568'),
('Karim Ali', '01123456791', 'karim.ali@email.com', '29502031234569'),
('Laila Hassan', '01098765434', 'laila.h@email.com', '29503041234570'),
('Youssef Mahmoud', '01234567893', 'youssef.m@email.com', '29501051234571'),
('Dina Omar', '01123456792', 'dina.o@email.com', '29502061234572'),
('Amr Samir', '01098765435', 'amr.s@email.com', '29503071234573'),
('Rania Khaled', '01234567894', 'rania.k@email.com', '29501081234574'),
('Tarek Nadia', '01123456793', 'tarek.n@email.com', '29502091234575');

-- Add more Housing Units with variety
INSERT INTO dbo.housing_unit (architectural_style, longitude, latitude, market_valuation, status, repres_id) VALUES
('Mediterranean Villa', 31.2200, 30.0300, 4200000.00, 'available', 4),
('Modern Apartment', 31.2500, 30.0600, 1500000.00, 'under offer', 5),
('Luxury Penthouse', 31.2100, 30.0200, 5500000.00, 'available', 6),
('Cozy Studio', 31.2600, 30.0700, 950000.00, 'sold', 7),
('Family Duplex', 31.2000, 30.0100, 2800000.00, 'available', 8),
('Beach House', 31.2700, 30.0800, 6800000.00, 'under offer', 4),
('City Loft', 31.2300, 30.0400, 1100000.00, 'available', 5),
('Garden Villa', 31.2400, 30.0500, 3800000.00, 'sold', 6),
('Sky Apartment', 31.2800, 30.0900, 2200000.00, 'available', 7),
('Classic Townhouse', 31.2150, 30.0250, 3200000.00, 'under offer', 8);

-- Add more Tours
INSERT INTO dbo.tour (tour_date, tour_time, unit_id, repres_id) VALUES
('2026-05-17', '09:00:00', 7, 4),
('2026-05-17', '11:00:00', 8, 5),
('2026-05-17', '02:00:00', 9, 6),
('2026-05-18', '10:00:00', 10, 7),
('2026-05-18', '12:00:00', 11, 8),
('2026-05-18', '03:00:00', 12, 4),
('2026-05-19', '09:30:00', 13, 5),
('2026-05-19', '11:30:00', 14, 6),
('2026-05-19', '01:30:00', 15, 7),
('2026-05-20', '10:30:00', 16, 8);

-- Add more Client-Tour relationships
INSERT INTO dbo.client_tour (tour_id, client_id) VALUES
(5, 4), (5, 5), (6, 6), (6, 7), (7, 8), (7, 9), (8, 10), (8, 11),
(9, 12), (9, 13), (10, 14), (10, 15), (11, 16), (11, 17), (12, 18), (12, 19);

-- Verify final data counts
SELECT 'Data Summary:' as Info, '' as Count
UNION ALL
SELECT 'Representatives:', CAST(COUNT(*) AS VARCHAR) FROM dbo.representative
UNION ALL
SELECT 'Clients:', CAST(COUNT(*) AS VARCHAR) FROM dbo.client
UNION ALL
SELECT 'Housing Units:', CAST(COUNT(*) AS VARCHAR) FROM dbo.housing_unit
UNION ALL
SELECT 'Available Units:', CAST(COUNT(*) AS VARCHAR) FROM dbo.housing_unit WHERE status = 'available'
UNION ALL
SELECT 'Under Offer Units:', CAST(COUNT(*) AS VARCHAR) FROM dbo.housing_unit WHERE status = 'under offer'
UNION ALL
SELECT 'Sold Units:', CAST(COUNT(*) AS VARCHAR) FROM dbo.housing_unit WHERE status = 'sold'
UNION ALL
SELECT 'Tours:', CAST(COUNT(*) AS VARCHAR) FROM dbo.tour
UNION ALL
SELECT 'Client-Tour Relations:', CAST(COUNT(*) AS VARCHAR) FROM dbo.client_tour;
