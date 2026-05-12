-- Correct Sample Data with proper IDs
-- Use the actual IDs that exist in the database

-- Insert Tours with correct unit_ids (2, 3, 4, 5, 6)
INSERT INTO dbo.tour (tour_date, tour_time, unit_id, repres_id) VALUES
('2026-05-15', '10:00:00', 2, 1),  -- Modern Villa with Ahmed
('2026-05-15', '14:00:00', 3, 2),  -- Classic Apartment with Sara
('2026-05-16', '11:00:00', 4, 3),  -- Penthouse with Omar
('2026-05-16', '15:00:00', 6, 2);  -- Duplex with Sara

-- Create client-tour relationships
INSERT INTO dbo.client_tour (tour_id, client_id) VALUES
(1, 1),  -- Mohamed Salah with Tour 1
(2, 2),  -- Fatima Ali with Tour 2
(3, 3),  -- Youssef Omar with Tour 3
(4, 1);  -- Mohamed Salah with Tour 4

-- Verify all data
SELECT 'Representatives:' as Info, CAST(COUNT(*) AS VARCHAR) as Count FROM dbo.representative
UNION ALL
SELECT 'Clients:', CAST(COUNT(*) AS VARCHAR) FROM dbo.client
UNION ALL
SELECT 'Housing Units:', CAST(COUNT(*) AS VARCHAR) FROM dbo.housing_unit
UNION ALL
SELECT 'Tours:', CAST(COUNT(*) AS VARCHAR) FROM dbo.tour
UNION ALL
SELECT 'Client-Tour Relations:', CAST(COUNT(*) AS VARCHAR) FROM dbo.client_tour;
