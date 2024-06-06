CREATE TABLE IF NOT EXISTS Appointments
(
    id serial ,
    email character varying,
    appointmenttime timestamp without time zone
)

SELECT * FROM appointments

Insert into appointments ( email,appointmenttime) values ('kirtan.patel@etatvasoft.com', '2024-06-07 12:07:16.889248+05:30'),('kirtan.patel@etatvasoft.com', '2024-06-08 12:07:16.889248+05:30')

--function for GetAppointmentDetails those have appointment in next 24 hrs
CREATE TYPE Records AS (
    id INT,
    email VARCHAR,
    appointmenttime TIMESTAMP WITHOUT TIME ZONE
);

CREATE OR REPLACE FUNCTION AppointmentDetails()
RETURNS SETOF Records
LANGUAGE plpgsql    
AS $$
DECLARE
    rec Records;
BEGIN
    FOR rec IN
        SELECT
			a.id AS id,
			a.email AS email,
			a.appointmenttime AS appointmenttime
        FROM 
            appointments AS a
        WHERE 
            EXTRACT(EPOCH FROM (a.appointmenttime - NOW())) / 3600 <= 24
    LOOP
        RETURN NEXT rec;
    END LOOP;
    
    RETURN;
END;
$$;

--call function        
SELECT * FROM AppointmentDetails();