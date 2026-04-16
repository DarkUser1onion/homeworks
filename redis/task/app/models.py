"""
SQLAlchemy модели — Author, Book, Borrowing.
"""

from __future__ import annotations

import datetime

from sqlalchemy import (
    Boolean,
    DateTime,
    Float,
    ForeignKey,
    Integer,
    String,
    Text,
    func,
)
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column, relationship


class Base(DeclarativeBase):
    pass


# ═══════════════════════════════════════════════════════════
# Author — справочник авторов
# ═══════════════════════════════════════════════════════════

class Author(Base):
    __tablename__ = "authors"

    id: Mapped[int] = mapped_column(Integer, primary_key=True, autoincrement=True)
    name: Mapped[str] = mapped_column(String(200), nullable=False)
    slug: Mapped[str] = mapped_column(String(200), nullable=False, unique=True, index=True)
    bio: Mapped[str] = mapped_column(Text, nullable=False, default="")

    books: Mapped[list["Book"]] = relationship(back_populates="author")

    def to_dict(self) -> dict:
        return {
            "id": self.id,
            "name": self.name,
            "slug": self.slug,
            "bio": self.bio,
        }


# ═══════════════════════════════════════════════════════════
# Book — каталог книг
# ═══════════════════════════════════════════════════════════

class Book(Base):
    __tablename__ = "books"

    id: Mapped[int] = mapped_column(Integer, primary_key=True, autoincrement=True)
    title: Mapped[str] = mapped_column(String(300), nullable=False)
    description: Mapped[str] = mapped_column(Text, nullable=False, default="")
    isbn: Mapped[str] = mapped_column(String(20), nullable=False, unique=True)
    author_id: Mapped[int] = mapped_column(
        Integer, ForeignKey("authors.id"), nullable=False, index=True,
    )
    year: Mapped[int] = mapped_column(Integer, nullable=False)
    rating: Mapped[float] = mapped_column(Float, nullable=False, default=0.0)
    views_count: Mapped[int] = mapped_column(Integer, nullable=False, default=0)
    is_available: Mapped[bool] = mapped_column(Boolean, nullable=False, default=True)
    created_at: Mapped[datetime.datetime] = mapped_column(
        DateTime, nullable=False, server_default=func.now(),
    )

    author: Mapped["Author"] = relationship(back_populates="books")
    borrowings: Mapped[list["Borrowing"]] = relationship(back_populates="book")

    def to_dict(self) -> dict:
        return {
            "id": self.id,
            "title": self.title,
            "description": self.description,
            "isbn": self.isbn,
            "author_id": self.author_id,
            "author_name": self.author.name if self.author else None,
            "year": self.year,
            "rating": self.rating,
            "views_count": self.views_count,
            "is_available": self.is_available,
            "created_at": self.created_at.isoformat() if self.created_at else None,
        }


# ═══════════════════════════════════════════════════════════
# Borrowing — выдача книг читателям
# ═══════════════════════════════════════════════════════════

class Borrowing(Base):
    __tablename__ = "borrowings"

    id: Mapped[int] = mapped_column(Integer, primary_key=True, autoincrement=True)
    reader_name: Mapped[str] = mapped_column(String(255), nullable=False)
    book_id: Mapped[int] = mapped_column(
        Integer, ForeignKey("books.id"), nullable=False, index=True,
    )
    borrowed_at: Mapped[datetime.datetime] = mapped_column(
        DateTime, nullable=False, server_default=func.now(),
    )
    returned_at: Mapped[datetime.datetime | None] = mapped_column(
        DateTime, nullable=True, default=None,
    )
    is_returned: Mapped[bool] = mapped_column(Boolean, nullable=False, default=False)

    book: Mapped["Book"] = relationship(back_populates="borrowings")

    def to_dict(self) -> dict:
        return {
            "id": self.id,
            "reader_name": self.reader_name,
            "book_id": self.book_id,
            "book_title": self.book.title if self.book else None,
            "borrowed_at": self.borrowed_at.isoformat() if self.borrowed_at else None,
            "returned_at": self.returned_at.isoformat() if self.returned_at else None,
            "is_returned": self.is_returned,
        }
