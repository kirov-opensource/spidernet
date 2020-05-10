CREATE TABLE public.task
(
    id bigint NOT NULL,
    template_id bigint,
    http_method character(7),
    parameters json,
    header json,
    parent_id bigint,
    user_id bigint NOT NULL,
    job_id bigint NOT NULL,
    stage character varying(20),
    uri text,
    execute_at date,
    complete_at date,
    fail_at date,
    result_id bigint,
    create_at date NOT NULL,
    delete_at date,
    update_at date NOT NULL,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
);

ALTER TABLE public.task
    OWNER to oallure;

CREATE TABLE public.template
(
    id bigint NOT NULL,
    no character varying(50) NOT NULL,
    header json,
    name character varying(50) NOT NULL,
    property_parsing_rule JSON NOT NULL,
    uri text NOT NULL,
    user_id bigint NOT NULL,
    create_at date NOT NULL,
    delete_at date,
    update_at date NOT NULL,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
);

ALTER TABLE public.template
    OWNER to oallure;

CREATE TABLE public.user
(
    id bigint NOT NULL,
    email character varying(80) NOT NULL,
    name character varying(50) NOT NULL,
    nick_name character varying(50) NOT NULL,
    password character varying(200) NOT NULL,
    mobile_number character varying(20) NOT NULL,
    token character varying(200) NOT NULL,
    create_at date NOT NULL,
    delete_at date,
    update_at date NOT NULL,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
);

ALTER TABLE public.user
    OWNER to oallure;