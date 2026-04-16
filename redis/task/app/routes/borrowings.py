from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.ext.asyncio import AsyncSession
from app.database import get_db
from app import crud, schemas
from app.cache_service import inv_book_borrowing

router = APIRouter(prefix="/borrowings", tags=["borrowings"])

@router.post("/", response_model=schemas.Borrowing, status_code=201)
async def create_borrowing(borrowing_data: schemas.BorrowingCreate, db: AsyncSession = Depends(get_db)):
    borrowing = await crud.create_borrowing(db, borrowing_data)
    await inv_book_borrowing(borrowing_data.book_id)
    return borrowing

@router.post("/{borrowing_id}/return", response_model=schemas.Borrowing)
async def return_book(borrowing_id: int, db: AsyncSession = Depends(get_db)):
    borrowing = await crud.return_book(db, borrowing_id)
    if not borrowing:
        raise HTTPException(status_code=404, detail="Borrowing not found")
    await inv_book_borrowing(borrowing.book_id)
    return borrowing

@router.get("/", response_model=list[schemas.Borrowing])
async def get_borrowings(db: AsyncSession = Depends(get_db)):
    return await crud.get_borrowings(db)

@router.get("/{borrowing_id}", response_model=schemas.Borrowing)
async def get_borrowing(borrowing_id: int, db: AsyncSession = Depends(get_db)):
    borrowing = await crud.get_borrowing(db, borrowing_id)
    if not borrowing:
        raise HTTPException(status_code=404, detail="Borrowing not found")
    return borrowing
