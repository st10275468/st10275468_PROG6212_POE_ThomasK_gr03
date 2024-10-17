CREATE SCHEMA store;
USE store;
CREATE TABLE employees(
id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
name VARCHAR(100),
email VARCHAR(100),
salary DECIMAL
);
CREATE TABLE products(
productid INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
productname VARCHAR(100),
category VARCHAR(100),
price DECIMAL
);
CREATE TABLE customers(
customerid INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
customername VARCHAR(100),
customeremail VARCHAR(100),
phone VARCHAR(100)
);
CREATE TABLE orders(
orderid INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
customerid INT,
productid INT,
quantity INT NOT NULL,
status	VARCHAR(100),
FOREIGN KEY (customerid)
REFERENCES customers(customerid),
FOREIGN KEY (productid)
REFERENCES products(productid)
);

INSERT INTO employees VALUES
(1, 'Jeff', 'jeff@email', 1000),
(2, 'John', 'john@email', 2000),
(3, 'Tom', 'Tom@email', 3000),
(4, 'James', 'James@email', 4000),
(5, 'Ethan', 'Ethan@email', 500);

INSERT INTO products VALUES
(1, 'Tomato', 'Fruit', 10),
(2, 'Orange', 'Fruit', 10),
(3, 'apple', 'Fruit', 10),
(4, 'Milk', 'Dairy', 30),
(5, 'Yoghurt', 'Dairy', 40);

INSERT INTO customers VALUES
(1, 'Andrew', 'Andrew@email', '123123213'),
(2, 'Olivia', 'Olivia@email', '123123123'),
(3, 'Adam', 'Adam@email', '123123123'),
(4, 'Jenna', 'Jenna@email', '123123123'),
(5, 'Mary', 'Mary@email', '1213212312');

INSERT INTO orders VALUES
(1, 2, 'Confirmed'),
(2, 3, 'processing'),
(3, 4, 'Deliverd'),
(4, 1, 'Pending'),
(5, 2, 'Confirmed');

SELECT *
FROM employees LIMIT 3;

SELECT *
FROM products 
WHERE productname = 'apple';

SELECT *
FROM customers 
WHERE customeremail = null;


SELECT *
FROM orders 
WHERE status IN ('Pending','processing');

SELECT *
FROM employees 
WHERE EXISTS(
SELECT *
FROM orders
WHERE orders.status = 'Confirmed'
);

SELECT *
FROM sales 
WHERE date >= '2023-01-01'
	AND date <= '2023-12-31';
