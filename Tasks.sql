--v1
CREATE OR REPLACE PROCEDURE SearchRecord(
   patientname IN VARCHAR(50),
   result_firstname OUT VARCHAR(50)
)
LANGUAGE plpgsql    
AS $$
DECLARE
BEGIN
    SELECT 
        r.firstname
    INTO 
        result_firstname
    FROM 
        request r
    LEFT JOIN 
        requestclient rc ON r.requestid = rc.requestid 
    WHERE 
        r.firstname LIKE '%' || patientname || '%'; 	
END;
$$;

-- Calling of SP 
CALL SearchRecord('ki', null);


--v2
CREATE OR REPLACE PROCEDURE SearchRecord1(
	statusid IN INT,
	spatientname IN VARCHAR,
	requesttype IN INT,
	fromDOS IN TIMESTAMP,
	toDOS IN TIMESTAMP,
	providername IN VARCHAR,
	emailid IN VARCHAR,
	mobile IN VARCHAR,
	reqid OUT INT,
	patientName OUT VARCHAR,
	requestor OUT INT,
	dateofservice OUT TIMESTAMP,
	closedate OUT TIMESTAMP,
	email OUT VARCHAR,
	mobileno OUT VARCHAR,
	address OUT VARCHAR,
	zip OUT VARCHAR,
	reqstatus OUT INT,
	physician OUT VARCHAR,
	phynotes OUT VARCHAR,
	adminnotes OUT VARCHAR,
	patientnotes OUT VARCHAR
)
LANGUAGE plpgsql    
AS $$
BEGIN
    SELECT 
        r.requestid,
        rc.firstname || ' ' || rc.lastname,
        r.requesttypeid,
        r.createddate,
        r.lastreservationdate,
        rc.email,
        rc.phonenumber,
        rc.address,
        rc.zipcode,
        r.status,
        p.firstname || ' ' || p.lastname,
        rn.physiciannotes,
        rn.adminnotes,
        rc.notes
    INTO 
        reqid,
        patientName,
        requestor,
        dateofservice,
        closedate,
        email,
        mobileno,
        address,
        zip,
        reqstatus,
        physician,
        phynotes,
        adminnotes,
        patientnotes
    FROM 
        request AS r
    LEFT JOIN 
        requestclient AS rc ON r.requestid = rc.requestid
    LEFT JOIN 
        physician AS p ON r.physicianid = p.physicianid
    LEFT JOIN 
        requestnotes AS rn ON r.requestid = rn.requestid
    WHERE 
        (statusid = 0 OR r.status = statusid)
		AND (spatientname IS NULL OR LOWER(rc.firstname || ' ' || rc.lastname) LIKE '%' || LOWER(spatientname) || '%' )
		AND (requesttype = 0 OR r.requesttypeid = requesttype)
        AND (fromDOS IS NULL OR toDOS IS NULL OR r.createddate::date BETWEEN fromDOS AND toDOS)
        AND (providername IS NULL OR LOWER(p.firstname || ' ' || p.lastname) LIKE '%' || LOWER(providername) || '%')
        AND (emailid IS NULL OR LOWER(rc.email) LIKE '%' || LOWER(emailid) || '%')
        AND (mobileno IS NULL OR rc.phonenumber LIKE '%' || mobileno || '%')
		AND (r.isdeleted IS NULL OR r.isdeleted = B'0');
        
END;
$$;
CALL SearchRecord1(0, NULL, 0, NULL, NULL, 'y', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);


--v3 SP returns LIST
CREATE OR REPLACE PROCEDURE SearchRecord1(
    statusid INT,
    spatientname VARCHAR,
    requesttype INT,
    fromDOS TIMESTAMP,
    toDOS TIMESTAMP,
    providername VARCHAR,
    emailid VARCHAR,
    mobilephone VARCHAR
)
RETURN SETOF RecordObject
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
            r.lastreservationdate AS closedate,
            rc.email,
            rc.phonenumber AS mobileno,
            rc.address,
            rc.zipcode AS zip,
            r.status AS reqstatus,
            p.firstname || ' ' || p.lastname AS physician,
            rn.physiciannotes AS phynotes,
            rn.adminnotes AS adminnotes,
            rc.notes AS patientnotes
        FROM 
            request AS r
        LEFT JOIN 
            requestclient AS rc ON r.requestid = rc.requestid
        LEFT JOIN 
            physician AS p ON r.physicianid = p.physicianid
        LEFT JOIN 
            requestnotes AS rn ON r.requestid = rn.requestid
        WHERE 
            (statusid = 0 OR r.status = statusid)
            AND (spatientname IS NULL OR LOWER(rc.firstname || ' ' || rc.lastname) LIKE '%' || LOWER(spatientname) || '%')
            AND (requesttype = 0 OR r.requesttypeid = requesttype)
            AND (fromDOS IS NULL OR toDOS IS NULL OR r.createddate::date BETWEEN fromDOS AND toDOS)
            AND (providername IS NULL OR LOWER(p.firstname || ' ' || p.lastname) LIKE '%' || LOWER(providername) || '%')
            AND (emailid IS NULL OR LOWER(rc.email) LIKE '%' || LOWER(emailid) || '%')
            AND (mobilephone IS NULL OR rc.phonenumber LIKE '%' || mobilephone || '%')
            AND (r.isdeleted IS NULL OR r.isdeleted = B'0')
    LOOP
        RETURN NEXT rec;
    END LOOP;
    
    RETURN;
END;
$$;

CALL SearchRecord1(0, NULL, 0, NULL, NULL, 'y', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);


--sql query logic
SELECT 
        r.requestid,
        rc.firstname || ' ' || rc.lastname,
        r.requesttypeid,
        r.createddate,
        r.lastreservationdate,
        rc.email,
        rc.phonenumber,
        rc.address,
        rc.zipcode,
        r.status,
        p.firstname || ' ' || p.lastname,
        rn.physiciannotes,
        rn.adminnotes,
        rc.notes
    
    FROM 
        request AS r
    LEFT JOIN 
        requestclient AS rc ON r.requestid = rc.requestid
    LEFT JOIN 
        physician AS p ON r.physicianid = p.physicianid
    LEFT JOIN 
        requestnotes AS rn ON r.requestid = rn.requestid
		
		
		
--final version
CREATE TYPE RecordObject AS (
    reqid INT,
    patientName VARCHAR,
    requestor INT,
    dateofservice TIMESTAMP,
    closedate TIMESTAMP,
    email VARCHAR,
    mobileno VARCHAR,
    address VARCHAR,
    zip VARCHAR,
    reqstatus INT,
    physician VARCHAR,
    phynotes VARCHAR,
    adminnotes VARCHAR,
    patientnotes VARCHAR
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
        SELECT DISTINCT 
            r.requestid AS reqid,
            rc.firstname || ' ' || rc.lastname AS patientName,
            r.requesttypeid AS requestor,
            r.createddate AS dateofservice,
            r.lastreservationdate AS closedate,
            rc.email,
            rc.phonenumber AS mobileno,
            rc.address,
            rc.zipcode AS zip,
            r.status AS reqstatus,
            p.firstname || ' ' || p.lastname AS physician/*,
            rn.physiciannotes AS phynotes,
            rn.adminnotes AS adminnotes,
            rc.notes AS patientnotes*/
        FROM 
            request AS r
         LEFT JOIN 
            requestclient AS rc ON r.requestid = rc.requestid
         LEFT JOIN 
            physician AS p ON r.physicianid = p.physicianid
         /*JOIN 
            requestnotes AS rn ON r.requestid = rn.requestid*/
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

