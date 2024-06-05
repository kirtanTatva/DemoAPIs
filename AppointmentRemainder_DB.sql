CREATE TABLE IF NOT EXISTS Appointments
(
    id serial ,
    email character varying,
    appointmenttime timestamp without time zone
)

SELECT * FROM appointments

Insert into appointments ( email,appointmenttime) values ('kirtan.patel@etatvasoft.com', '2024-06-07 12:07:16.889248+05:30'),('kirtan.patel@etatvasoft.com', '2024-06-08 12:07:16.889248+05:30')