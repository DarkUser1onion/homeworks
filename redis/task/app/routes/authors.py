from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.ext.asyncio import AsyncSession
from app.database import get_db
from app import crud, schemas
from app.cache_service import (
    get_authors_cache, set_authors_cache,
    get_author_cache, set_author_cache,
    inv_author
)

router = APIRouter(prefix="/authors", tags=["authors"])

@router.get("/", response_model=list[schemas.Author])
async def get_authors(db: AsyncSession = Depends(get_db)):
    cached = await get_authors_cache()
    if cached:
        return cached
    authors = await crud.get_authors(db)
    await set_authors_cache(authors)
    return authors

@router.get("/{author_id}", response_model=schemas.Author)
async def get_author(author_id: int, db: AsyncSession = Depends(get_db)):
    cached = await get_author_cache(author_id)
    if cached:
        return cached
    author = await crud.get_author(db, author_id)
    if not author:
        raise HTTPException(status_code=404, detail="Author not found")
    await set_author_cache(author_id, author)
    return author
