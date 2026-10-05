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
