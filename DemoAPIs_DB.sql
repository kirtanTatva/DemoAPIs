CREATE TABLE IF NOT EXISTS users
(
    id serial ,
    name integer,
    email character varying,
    password character varying,
    refreshToken character varying,
    expiresAt timestamp without time zone
)

CREATE TABLE IF NOT EXISTS items
(
    id serial ,
    name character varying,
    userid integer
)

Select * from users

Select * from items