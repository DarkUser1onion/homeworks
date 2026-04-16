"""
Pydantic-схемы для валидации запросов и ответов.
"""

from __future__ import annotations

from pydantic import BaseModel, Field


# ── Author ────────────────────────────────────────────────

class AuthorRead(BaseModel):
    id: int
    name: str
    slug: str
    bio: str
    model_config = {"from_attributes": True}


# ── Book ──────────────────────────────────────────────────

class BookRead(BaseModel):
    id: int
    title: str
    description: str
    isbn: str
    author_id: int
    author_name: str | None = None
    year: int
    rating: float
    views_count: int
    is_available: bool
    created_at: str | None = None


class BookCreate(BaseModel):
    title: str = Field(..., min_length=1, max_length=300)
    description: str = ""
    isbn: str = Field(..., min_length=10, max_length=20)
    author_id: int
    year: int = Field(..., ge=1000, le=2030)
    rating: float = Field(0.0, ge=0, le=5)


class BookUpdate(BaseModel):
    title: str | None = None
    description: str | None = None
    author_id: int | None = None
    year: int | None = Field(None, ge=1000, le=2030)
    rating: float | None = Field(None, ge=0, le=5)
    is_available: bool | None = None


# ── Borrowing ─────────────────────────────────────────────

class BorrowingCreate(BaseModel):
    reader_name: str = Field(..., min_length=1)
    book_id: int


class BorrowingRead(BaseModel):
    id: int
    reader_name: str
    book_id: int
    book_title: str | None = None
    borrowed_at: str | None = None
    returned_at: str | None = None
    is_returned: bool
