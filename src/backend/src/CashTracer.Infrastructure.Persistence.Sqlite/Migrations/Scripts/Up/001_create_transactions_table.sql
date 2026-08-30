CREATE TABLE IF NOT EXISTS transactions
(
    id INTEGER PRIMARY KEY,
    type INTEGER NOT NULL,
    concept TEXT NOT NULL,
    date TEXT NOT NULL,
    currency TEXT NOT NULL,
    amount TEXT NOT NULL,
    created_at TEXT NOT NULL,
    updated_at TEXT
);