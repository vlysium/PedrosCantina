-- Table definitions for the Pedros Cantina database

-- Employees table
CREATE TABLE [employees] (
    [employee_id]  INT            IDENTITY (1, 1) NOT NULL,
    [name]         NVARCHAR (63)  NOT NULL,
    [email]        NVARCHAR (255) NOT NULL,
    [phone_number] CHAR (8)       NOT NULL,
    CONSTRAINT [PK_employees] PRIMARY KEY CLUSTERED ([employee_id] ASC)
);

-- Managers table (inherits from employees)
CREATE TABLE [managers] (
    [manager_id] INT NOT NULL,
    CONSTRAINT [PK_managers] PRIMARY KEY CLUSTERED ([manager_id] ASC),
    CONSTRAINT [FK_managers_employees] FOREIGN KEY ([manager_id]) REFERENCES [employees] ([employee_id]) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Shift periods table (lookup table)
CREATE TABLE [shift_periods] (
    [period]     NVARCHAR (31) NOT NULL,
    [start_time] TIME (0)      NOT NULL,
    [end_time]   TIME (0)      NOT NULL,
    CONSTRAINT [PK_shift_periods] PRIMARY KEY CLUSTERED ([period] ASC)
);

-- Shifts table
CREATE TABLE [shifts] (
    [date]       DATE          NOT NULL,
    [period]     NVARCHAR (31) NOT NULL,
    [manager_id] INT           NOT NULL,
    CONSTRAINT [PK_shifts] PRIMARY KEY CLUSTERED ([date] ASC, [period] ASC),
    CONSTRAINT [FK_shifts_managers] FOREIGN KEY ([manager_id]) REFERENCES [managers] ([manager_id]),
    CONSTRAINT [FK_shifts_shift_periods] FOREIGN KEY ([period]) REFERENCES [shift_periods] ([period])
);

-- Employee shifts table (junction table, many-to-many relationship between employees and shifts)
CREATE TABLE [employee_shifts] (
    [employee_id]  INT           NOT NULL,
    [shift_date]   DATE          NOT NULL,
    [shift_period] NVARCHAR (31) NOT NULL,
    CONSTRAINT [PK_employee_shifts] PRIMARY KEY CLUSTERED ([employee_id] ASC, [shift_date] ASC, [shift_period] ASC),
    CONSTRAINT [FK_employee_shifts_employees] FOREIGN KEY ([employee_id]) REFERENCES [employees] ([employee_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_employee_shifts_shifts] FOREIGN KEY ([shift_date], [shift_period]) REFERENCES [shifts] ([date], [period]) ON DELETE CASCADE
);

-- View definitions

-- Manager details view (result of joining managers and employees to get manager details)
CREATE VIEW [vw_manager_details] AS
    SELECT m.manager_id, e.name, e.email, e.phone_number
    FROM managers AS m
    JOIN employees AS e ON e.employee_id = m.manager_id;

-- Shift details view (result of joining shifts, shift_periods, managers, employees, and employee_shifts to get detailed shift information)
CREATE VIEW [vw_shift_details] AS
    SELECT
        s.[date] AS shift_date, sp.period, sp.start_time, sp.end_time,
        m.manager_id, manager.name AS manager_name, manager.email AS manager_email, manager.phone_number AS manager_phone_number,
        e.employee_id, e.name AS employee_name, e.email AS employee_email, e.phone_number AS employee_phone_number
    FROM shifts AS s
    INNER JOIN shift_periods AS sp ON s.period = sp.period
    INNER JOIN managers AS m ON s.manager_id = m.manager_id
    INNER JOIN employees AS manager ON m.manager_id = manager.employee_id
    INNER JOIN employee_shifts AS es ON s.[date] = es.shift_date AND s.period = es.shift_period
    INNER JOIN employees AS e ON es.employee_id = e.employee_id;
