CREATE TABLE IF NOT EXISTS users
(
    id serial ,
    name integer,
    email character varying,
    password character varying,
    refreshToken character varying,
    expiresAt timestamp without time zone
)

Select * from users