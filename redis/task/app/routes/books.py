from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.ext.asyncio import AsyncSession
from app.database import get_db
from app import crud, schemas
from app.cache_service import (
    get_book_cache, set_book_cache, inv_book,
    get_top_rated_cache, set_top_rated_cache,
    get_popular_cache, set_popular_cache,
    get_books_count_cache, set_books_count_cache,
    get_books_list_cache, set_books_list_cache,
    inv_books_list, inv_top_rated, inv_popular, inv_books_count
)

router = APIRouter(prefix="/books", tags=["books"])

@router.get("/", response_model=list[schemas.Book])
async def get_books(
    page: int = Query(1, ge=1),
    size: int = Query(20, ge=1, le=100),
    author_id: int | None = None,
    db: AsyncSession = Depends(get_db)
):
    if page == 1 and size == 20:
        key = f"books:list:page1_size20_author_{author_id}"
        cached = await get_books_list_cache(key)
        if cached:
            return cached
        books = await crud.get_books(db, page=page, size=size, author_id=author_id)
        await set_books_list_cache(key, books)
        return books
    return await crud.get_books(db, page=page, size=size, author_id=author_id)

@router.get("/top-rated", response_model=list[schemas.Book])
async def get_top_rated_books(db: AsyncSession = Depends(get_db)):
    cached = await get_top_rated_cache()
    if cached:
        return cached
    books = await crud.get_top_rated_books(db)
    await set_top_rated_cache(books)
    return books

@router.get("/popular", response_model=list[schemas.Book])
async def get_popular_books(db: AsyncSession = Depends(get_db)):
    cached = await get_popular_cache()
    if cached:
        return cached
    books = await crud.get_popular_books(db)
    await set_popular_cache(books)
    return books

@router.get("/count", response_model=schemas.BookCount)
async def get_books_count(db: AsyncSession = Depends(get_db)):
    cached = await get_books_count_cache()
    if cached:
        return cached
    count = await crud.get_books_count(db)
    result = {"count": count}
    await set_books_count_cache(result)
    return result

@router.get("/search", response_model=list[schemas.Book])
async def search_books(q: str, db: AsyncSession = Depends(get_db)):
    return await crud.search_books(db, q)

@router.get("/{book_id}", response_model=schemas.Book)
async def get_book(book_id: int, db: AsyncSession = Depends(get_db)):
    cached = await get_book_cache(book_id)
    if cached:
        return cached
    book = await crud.get_book(db, book_id)
    if not book:
        raise HTTPException(status_code=404, detail="Book not found")
    await crud.increment_views(db, book_id)
    book = await crud.get_book(db, book_id)
    await set_book_cache(book_id, book)
    return book

@router.post("/", response_model=schemas.Book, status_code=201)
async def create_book(book_data: schemas.BookCreate, db: AsyncSession = Depends(get_db)):
    book = await crud.create_book(db, book_data)
    await inv_books_list()
    await inv_books_count()
    await inv_top_rated()
    await inv_popular()
    return book

@router.patch("/{book_id}", response_model=schemas.Book)
async def update_book(book_id: int, book_data: schemas.BookUpdate, db: AsyncSession = Depends(get_db)):
    book = await crud.update_book(db, book_id, book_data)
    if not book:
        raise HTTPException(status_code=404, detail="Book not found")
    await inv_book(book_id)
    return book
