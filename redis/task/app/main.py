from contextlib import asynccontextmanager
from fastapi import FastAPI
from app.database import engine
from app import models
from app.authors import router as authors_router
from app.books import router as books_router
from app.borrowings import router as borrowings_router
from app.redis_client import get_redis, close_redis

models.Base.metadata.create_all(bind=engine)

@asynccontextmanager
async def lifespan(app: FastAPI):
    await get_redis()
    print("redis started")
    yield
    await close_redis()
    print("redis stopped")

app = FastAPI(lifespan=lifespan)

app.include_router(authors_router)
app.include_router(books_router)
app.include_router(borrowings_router)

@app.get("/health")
async def health():
    return {"status": "ok", "cache": "redis"}
