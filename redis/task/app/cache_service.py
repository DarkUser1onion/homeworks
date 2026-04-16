import json
import random
from app.redis_client import get_redis

DEFAULT_TTL = 300

def add_jitter(ttl):
    return ttl + int(ttl * 0.1 * random.random())

async def cache_get(key):
    r = await get_redis()
    data = await r.get(key)
    if data:
        try:
            return json.loads(data)
        except:
            return None
    return None

async def cache_set(key, value, ttl=DEFAULT_TTL):
    r = await get_redis()
    ttl = add_jitter(ttl)
    await r.setex(key, ttl, json.dumps(value, default=str))

async def cache_delete(key):
    r = await get_redis()
    await r.delete(key)

async def cache_delete_pattern(pattern):
    r = await get_redis()
    cur = 0
    while True:
        cur, keys = await r.scan(cur, match=pattern, count=100)
        if keys:
            await r.delete(*keys)
        if cur == 0:
            break

async def get_authors_cache():
    return await cache_get("authors:list")

async def set_authors_cache(authors):
    await cache_set("authors:list", authors, 3600)

async def inv_authors_list():
    await cache_delete("authors:list")

async def get_author_cache(aid):
    return await cache_get(f"author:{aid}")

async def set_author_cache(aid, data):
    await cache_set(f"author:{aid}", data, 86400)

async def inv_author(aid):
    await cache_delete(f"author:{aid}")
    await inv_authors_list()

async def get_book_cache(bid):
    return await cache_get(f"book:{bid}")

async def set_book_cache(bid, data):
    await cache_set(f"book:{bid}", data, 180)

async def inv_book(bid):
    await cache_delete(f"book:{bid}")
    await inv_books_list()
    await inv_top_rated()
    await inv_popular()
    await inv_books_count()

async def get_books_list_cache(key):
    return await cache_get(key)

async def set_books_list_cache(key, books):
    await cache_set(key, books, 120)

async def inv_books_list():
    await cache_delete_pattern("books:list:*")

async def get_top_rated_cache():
    return await cache_get("books:top-rated")

async def set_top_rated_cache(books):
    await cache_set("books:top-rated", books, 300)

async def inv_top_rated():
    await cache_delete("books:top-rated")

async def get_popular_cache():
    return await cache_get("books:popular")

async def set_popular_cache(books):
    await cache_set("books:popular", books, 60)

async def inv_popular():
    await cache_delete("books:popular")

async def get_books_count_cache():
    return await cache_get("books:count")

async def set_books_count_cache(cnt):
    await cache_set("books:count", cnt, 300)

async def inv_books_count():
    await cache_delete("books:count")

async def inv_book_borrowing(bid):
    await inv_book(bid)
