import redis.asyncio as redis

redis_conn = None

async def get_redis():
    global redis_conn
    if redis_conn is None:
        from app.config import settings
        redis_conn = redis.from_url(settings.redis_url, encoding="utf-8", decode_responses=True)
        await redis_conn.ping()
        print("redis ok")
    return redis_conn

async def close_redis():
    global redis_conn
    if redis_conn:
        await redis_conn.close()
        redis_conn = None
        print("redis closed")
