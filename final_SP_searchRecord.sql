--final version
CREATE TYPE RecordObject AS (
    reqid INT,
    patientName VARCHAR,
    requestor INT,
    dateofservice TIMESTAMP,
    closecasedate TIMESTAMP,
    email VARCHAR,
    mobileno VARCHAR,
    address VARCHAR,
    zip VARCHAR,
    RequestStatus INT,
    physician VARCHAR,
    PhysicianNote VARCHAR,
    AdminNote VARCHAR,
    PatientNote VARCHAR
);

CREATE OR REPLACE FUNCTION SearchRecord(
    statusid INT default 0,
    spatientname VARCHAR default null,
    requesttype INT default 0,
    fromDOS TIMESTAMP default null,
    toDOS TIMESTAMP default null,
    providername VARCHAR default null,
    emailid VARCHAR default null,
    mobilephone VARCHAR default null,
	page INT default 0,
	pagesize INT default 3
)
RETURNS SETOF RecordObject
LANGUAGE plpgsql    
AS $$
DECLARE
    rec RecordObject;
BEGIN
    FOR rec IN
        SELECT 
            r.requestid AS reqid,
            rc.firstname || ' ' || rc.lastname AS patientName,
            r.requesttypeid AS requestor,
            r.createddate AS dateofservice,
            r.lastreservationdate AS closecasedate,
            rc.email,
            rc.phonenumber AS mobileno,
            rc.address,
            rc.zipcode AS zip,
            r.status AS RequestStatus,
            p.firstname || ' ' || p.lastname AS physician
        FROM 
            request AS r
         LEFT JOIN 
            requestclient AS rc ON r.requestid = rc.requestid
         LEFT JOIN 
            physician AS p ON r.physicianid = p.physicianid
        WHERE 
            (statusid = 0 OR r.status = statusid)
            AND (spatientname IS NULL OR LOWER(rc.firstname || ' ' || rc.lastname) LIKE '%' || LOWER(spatientname) || '%')
            AND (requesttype = 0 OR r.requesttypeid = requesttype)
            AND (fromDOS IS NULL OR toDOS IS NULL OR r.createddate::date BETWEEN fromDOS AND toDOS)
            AND (providername IS NULL OR LOWER(p.firstname || ' ' || p.lastname) LIKE '%' || LOWER(providername) || '%')
            AND (emailid IS NULL OR LOWER(rc.email) LIKE '%' || LOWER(emailid) || '%')
            AND (mobilephone IS NULL OR rc.phonenumber LIKE '%' || mobilephone || '%')
            AND (r.isdeleted IS NULL OR r.isdeleted = B'0')
			OFFSET (page * pagesize)
			LIMIT pagesize
    LOOP
        RETURN NEXT rec;
    END LOOP;
    
    RETURN;
END;
$$;

--call function        
SELECT * FROM SearchRecord(
    statusid := 0,
    spatientname := NULL,
    requesttype := 0,
    fromDOS := NULL,
    toDOS := NULL,
    providername := 'as',
    emailid := NULL,
    mobilephone := NULL,
	page := 0,
	pagesize := 100	
);
