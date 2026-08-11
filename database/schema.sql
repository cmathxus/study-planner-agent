create table if not exists study_topics (
    id uuid primary key,
    name text not null,
    description text not null default ''
);

create table if not exists study_schedules (
    id uuid primary key,
    study_topic_id uuid not null references study_topics(id) on delete cascade,
    weekday integer not null check (weekday between 0 and 6)
);

create table if not exists study_progress_entries (
    id uuid primary key,
    study_topic_id uuid not null references study_topics(id) on delete cascade,
    studied_on date not null,
    percentage integer not null check (percentage between 20 and 100),
    notes text null,
    created_at timestamptz not null default now()
);

insert into study_topics (id, name, description)
values
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 'C# fundamentals', 'Review syntax, records, LINQ and async/await.'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 'ASP.NET Core', 'Practice controllers, minimal APIs, dependency injection and middleware.'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3', 'Clean Architecture', 'Review domain, application, infrastructure and API boundaries.'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4', 'Supabase/Postgres', 'Practice tables, SQL queries and repository adapters.'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5', 'MCP and Foundry', 'Study tools, MCP server transport and agent integration.'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6', 'Project practice', 'Build small features and write notes about tradeoffs.'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7', 'Weekly review', 'Review progress, gaps and next week focus.')
on conflict (id) do nothing;

insert into study_schedules (id, study_topic_id, weekday)
values
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 1),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 2),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3', 3),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4', 4),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5', 5),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6', 6),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7', 0)
on conflict (id) do nothing;
