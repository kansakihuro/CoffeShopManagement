/*
    Coffee Shop Management Website
    Database: CoffeeShopManagementDB
    SQL Server
    Run this script in SSMS using a login that can create databases.
*/

IF DB_ID(N'CoffeeShopManagementDB') IS NULL
BEGIN
    CREATE DATABASE CoffeeShopManagementDB;
END
GO

USE CoffeeShopManagementDB;
GO

/* Re-runnable demo script: drop dependent tables first if they exist. */
IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID(N'dbo.Orders', N'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID(N'dbo.CafeTables', N'U') IS NOT NULL DROP TABLE dbo.CafeTables;
IF OBJECT_ID(N'dbo.Areas', N'U') IS NOT NULL DROP TABLE dbo.Areas;
IF OBJECT_ID(N'dbo.Staff', N'U') IS NOT NULL DROP TABLE dbo.Staff;
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID(N'dbo.Categories', N'U') IS NOT NULL DROP TABLE dbo.Categories;
GO

CREATE TABLE dbo.Categories
(
    CategoryID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(255) NULL,
    IsActive     BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),
    CONSTRAINT UQ_Categories_CategoryName UNIQUE (CategoryName)
);
GO

CREATE TABLE dbo.Products
(
    ProductID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
    CategoryID  INT NOT NULL,
    ProductName NVARCHAR(150) NOT NULL,
    Price       DECIMAL(12,2) NOT NULL,
    IsActive    BIT NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
    CONSTRAINT FK_Products_Categories
        FOREIGN KEY (CategoryID) REFERENCES dbo.Categories(CategoryID),
    CONSTRAINT CK_Products_Price CHECK (Price >= 0)
);
GO

CREATE TABLE dbo.Staff
(
    StaffID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Staff PRIMARY KEY,
    FullName  NVARCHAR(150) NOT NULL,
    Phone     NVARCHAR(20) NULL,
    Email     NVARCHAR(150) NULL,
    RoleName  NVARCHAR(20) NOT NULL CONSTRAINT DF_Staff_RoleName DEFAULT (N'Staff'),
    IsActive  BIT NOT NULL CONSTRAINT DF_Staff_IsActive DEFAULT (1),
    CONSTRAINT CK_Staff_RoleName CHECK (RoleName IN (N'Admin', N'Manager', N'Staff'))
);
GO

CREATE TABLE dbo.Areas
(
    AreaID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Areas PRIMARY KEY,
    AreaName NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Areas_AreaName UNIQUE (AreaName)
);
GO

CREATE TABLE dbo.CafeTables
(
    TableID     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CafeTables PRIMARY KEY,
    AreaID      INT NOT NULL,
    TableName   NVARCHAR(50) NOT NULL,
    TableStatus NVARCHAR(20) NOT NULL CONSTRAINT DF_CafeTables_TableStatus DEFAULT (N'Available'),
    CONSTRAINT FK_CafeTables_Areas
        FOREIGN KEY (AreaID) REFERENCES dbo.Areas(AreaID),
    CONSTRAINT CK_CafeTables_TableStatus
        CHECK (TableStatus IN (N'Available', N'Occupied', N'Inactive')),
    CONSTRAINT UQ_CafeTables_Area_TableName UNIQUE (AreaID, TableName)
);
GO

CREATE TABLE dbo.Orders
(
    OrderID     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
    StaffID     INT NOT NULL,
    TableID     INT NULL,
    OrderType   NVARCHAR(20) NOT NULL,
    OrderStatus NVARCHAR(20) NOT NULL CONSTRAINT DF_Orders_OrderStatus DEFAULT (N'New'),
    CreatedAt   DATETIME2(0) NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT (SYSDATETIME()),
    CompletedAt DATETIME2(0) NULL,
    Notes       NVARCHAR(255) NULL,
    CONSTRAINT FK_Orders_Staff
        FOREIGN KEY (StaffID) REFERENCES dbo.Staff(StaffID),
    CONSTRAINT FK_Orders_CafeTables
        FOREIGN KEY (TableID) REFERENCES dbo.CafeTables(TableID),
    CONSTRAINT CK_Orders_OrderType
        CHECK (OrderType IN (N'DineIn', N'TakeAway')),
    CONSTRAINT CK_Orders_OrderStatus
        CHECK (OrderStatus IN (N'New', N'Preparing', N'Completed', N'Cancelled')),
    CONSTRAINT CK_Orders_OrderType_Table
        CHECK ((OrderType = N'DineIn' AND TableID IS NOT NULL)
            OR (OrderType = N'TakeAway' AND TableID IS NULL)),
    CONSTRAINT CK_Orders_CompletedAt
        CHECK (OrderStatus <> N'Completed' OR CompletedAt IS NOT NULL)
);
GO

CREATE TABLE dbo.OrderDetails
(
    OrderDetailID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderDetails PRIMARY KEY,
    OrderID       INT NOT NULL,
    ProductID     INT NOT NULL,
    Quantity      INT NOT NULL,
    UnitPrice     DECIMAL(12,2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Orders
        FOREIGN KEY (OrderID) REFERENCES dbo.Orders(OrderID),
    CONSTRAINT FK_OrderDetails_Products
        FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID),
    CONSTRAINT CK_OrderDetails_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderDetails_UnitPrice CHECK (UnitPrice >= 0)
);
GO

/* Sample data */
INSERT INTO dbo.Categories (CategoryName, Description)
VALUES
(N'Cà phê', N'Các loại cà phê nóng và đá'),
(N'Trà', N'Trà trái cây và trà sữa'),
(N'Đá xay', N'Đồ uống đá xay');

INSERT INTO dbo.Products (CategoryID, ProductName, Price)
VALUES
(1, N'Cà phê đen đá', 25000),
(1, N'Cà phê sữa đá', 30000),
(1, N'Bạc xỉu', 32000),
(2, N'Trà đào cam sả', 35000),
(2, N'Trà vải', 33000),
(3, N'Chocolate đá xay', 45000);

INSERT INTO dbo.Staff (FullName, Phone, Email, RoleName)
VALUES
(N'Nguyễn Minh Anh', N'0900000001', N'minhanh@example.com', N'Manager'),
(N'Trần Gia Bảo', N'0900000002', N'giabao@example.com', N'Staff');

INSERT INTO dbo.Areas (AreaName)
VALUES (N'Tầng trệt'), (N'Tầng lầu');

INSERT INTO dbo.CafeTables (AreaID, TableName, TableStatus)
VALUES
(1, N'Bàn 01', N'Available'),
(1, N'Bàn 02', N'Available'),
(1, N'Bàn 03', N'Available'),
(2, N'Bàn 04', N'Available');

GO

/* Demo order: one completed takeaway order for testing revenue reports. */
INSERT INTO dbo.Orders (StaffID, TableID, OrderType, OrderStatus, CreatedAt, CompletedAt, Notes)
VALUES (2, NULL, N'TakeAway', N'Completed', SYSDATETIME(), SYSDATETIME(), N'Dữ liệu mẫu kiểm thử doanh thu');

DECLARE @DemoOrderID INT = SCOPE_IDENTITY();

INSERT INTO dbo.OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
VALUES
(@DemoOrderID, 1, 2, 25000),
(@DemoOrderID, 4, 1, 35000);
GO

/* Verify tables and sample data */
SELECT * FROM dbo.Categories;
SELECT * FROM dbo.Products;
SELECT * FROM dbo.Staff;
SELECT * FROM dbo.Areas;
SELECT * FROM dbo.CafeTables;
SELECT * FROM dbo.Orders;
SELECT * FROM dbo.OrderDetails;

/* Revenue by completion date: only Completed orders count. */
SELECT
    CAST(CompletedAt AS DATE) AS RevenueDate,
    SUM(od.Quantity * od.UnitPrice) AS Revenue
FROM dbo.Orders AS o
INNER JOIN dbo.OrderDetails AS od ON od.OrderID = o.OrderID
WHERE o.OrderStatus = N'Completed'
GROUP BY CAST(CompletedAt AS DATE)
ORDER BY RevenueDate DESC;
GO
