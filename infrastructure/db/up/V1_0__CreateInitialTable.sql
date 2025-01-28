CREATE TABLE todo."Todos" (
    "Id" uuid NOT NULL,
    "Title" text NOT NULL,
    "Description" text NOT NULL,
    "DoneAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "RemovedAt" timestamp with time zone,
    CONSTRAINT "PK_Todos" PRIMARY KEY ("Id")
);
