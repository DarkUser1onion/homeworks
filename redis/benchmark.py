"""
╔══════════════════════════════════════════════════════════════════════╗
║           BENCHMARK: Library API — замер производительности          ║
║                                                                      ║
║   Тест 1a–1c: Latency — 100 последовательных запросов               ║
║     1a) GET /books/1         — карточка книги                        ║
║     1b) GET /authors/        — список авторов                        ║
║     1c) GET /books/top-rated — топ по рейтингу                       ║
║                                                                      ║
║   Тест 2: Нагрузка    —  50 параллельных пользователей, 2000 req     ║
║   Тест 3: Тяж. нагрузка — 100 параллельных пользователей, 5000 req  ║
╚══════════════════════════════════════════════════════════════════════╝

Запуск:
    cd task && docker compose up -d --build   (порт 8000)

    pip install aiohttp
    python benchmark.py
"""

import asyncio
import time
import statistics
import random
import sys

try:
    import aiohttp
except ImportError:
    print("pip install aiohttp")
    sys.exit(1)


# ─── Config ──────────────────────────────────────────────────────

BASE_URL = "http://localhost:8010"

ENDPOINTS = [
    ("/authors/",                 "GET /authors/"),
    ("/authors/1",                "GET /authors/1"),
    ("/authors/3",                "GET /authors/3"),
    ("/books/top-rated?limit=10", "GET /top-rated"),
    ("/books/popular?limit=10",   "GET /popular"),
    ("/books/1",                  "GET /books/1"),
    ("/books/5",                  "GET /books/5"),
    ("/books/18",                 "GET /books/18"),
    ("/books/?offset=0&limit=20", "GET /books/ page"),
    ("/books/count",              "GET /books/count"),
    ("/books/search?q=мир",       "GET /books/search"),
    ("/borrowings/",              "GET /borrowings/"),
]

# Веса для weighted-random в параллельных тестах (имитируем реальный трафик)
WEIGHTS = [18, 10, 8, 15, 15, 10, 8, 8, 8, 4, 3, 3]


# ─── Helpers ─────────────────────────────────────────────────────

async def single_get(session: aiohttp.ClientSession, url: str) -> float:
    """Один GET-запрос, возвращает время ответа в секундах."""
    start = time.perf_counter()
    async with session.get(url) as resp:
        await resp.read()
    return time.perf_counter() - start


async def sequential_latency_test(path: str, n: int) -> list[float]:
    """n последовательных GET-запросов — без параллелизма, чистая задержка."""
    latencies = []
    async with aiohttp.ClientSession() as session:
        await single_get(session, f"{BASE_URL}{path}")  # прогрев
        for _ in range(n):
            lat = await single_get(session, f"{BASE_URL}{path}")
            latencies.append(lat)
    return latencies


async def parallel_load(concurrency: int, total: int) -> tuple[dict[str, list[float]], float]:
    """Параллельная нагрузка — weighted random по эндпоинтам."""
    results: dict[str, list[float]] = {}
    lock = asyncio.Lock()
    sem = asyncio.Semaphore(concurrency)
    paths = [e[0] for e in ENDPOINTS]
    names = [e[1] for e in ENDPOINTS]

    async def do_request(session: aiohttp.ClientSession):
        async with sem:
            idx = random.choices(range(len(paths)), weights=WEIGHTS, k=1)[0]
            lat = await single_get(session, f"{BASE_URL}{paths[idx]}")
            async with lock:
                results.setdefault(names[idx], []).append(lat)

    connector = aiohttp.TCPConnector(limit=concurrency * 2)
    t0 = time.perf_counter()
    async with aiohttp.ClientSession(connector=connector) as session:
        await asyncio.gather(*[do_request(session) for _ in range(total)])
    elapsed = time.perf_counter() - t0

    return results, elapsed


# ─── Pretty print ─────────────────────────────────────────────────

def ms(val: float) -> str:
    return f"{val * 1000:.2f}"

def pct(sorted_list: list[float], p: float) -> float:
    idx = int(len(sorted_list) * p)
    return sorted_list[min(idx, len(sorted_list) - 1)]

def banner(text: str):
    print()
    print("━" * 68)
    print(f"  {text}")
    print("━" * 68)


def print_latency(label: str, lats: list[float]):
    s = sorted(lats)
    print(f"\n  {label}")
    print("  ┌──────────────────┬──────────────┐")
    print("  │ Метрика          │    Latency   │")
    print("  ├──────────────────┼──────────────┤")
    for metric, val in [
        ("Avg",    statistics.mean(lats)),
        ("Median", statistics.median(lats)),
        ("P95",    pct(s, 0.95)),
        ("P99",    pct(s, 0.99)),
        ("Min",    min(lats)),
        ("Max",    max(lats)),
    ]:
        print(f"  │ {metric:<16} │ {ms(val):>9} ms │")
    print("  └──────────────────┴──────────────┘")


def print_load(label: str, data: dict[str, list[float]], elapsed: float, total: int):
    all_lats = [lat for lats in data.values() for lat in lats]
    rps = total / elapsed
    s = sorted(all_lats)

    print(f"\n  {label}")
    print("  ┌──────────────────────────────┬──────────────┐")
    print("  │ Метрика                      │   Значение   │")
    print("  ├──────────────────────────────┼──────────────┤")
    print(f"  │ Throughput (RPS)             │ {rps:>8.0f} r/s │")
    print(f"  │ Total time                   │ {elapsed:>9.2f}  s │")
    print(f"  │ Avg latency                  │ {statistics.mean(all_lats)*1000:>9.2f} ms │")
    print(f"  │ Median latency               │ {statistics.median(all_lats)*1000:>9.2f} ms │")
    print(f"  │ P95 latency                  │ {pct(s, 0.95)*1000:>9.2f} ms │")
    print(f"  │ P99 latency                  │ {pct(s, 0.99)*1000:>9.2f} ms │")
    print("  └──────────────────────────────┴──────────────┘")

    print(f"\n  По эндпоинтам:")
    print(f"  {'Endpoint':<26} {'Запросов':>8}  {'Avg':>9}  {'P95':>9}")
    print(f"  {'─' * 58}")
    for name, lats in sorted(data.items()):
        avg = statistics.mean(lats) * 1000
        p95 = pct(sorted(lats), 0.95) * 1000
        print(f"  {name:<26} {len(lats):>8}  {avg:>7.2f}ms  {p95:>7.2f}ms")


# ─── Main ─────────────────────────────────────────────────────────

async def main():
    print()
    print("╔══════════════════════════════════════════════════════════════╗")
    print("║         📚 BENCHMARK: Library API                            ║")
    print("╚══════════════════════════════════════════════════════════════╝")

    # Проверяем доступность
    try:
        async with aiohttp.ClientSession() as s:
            async with s.get(f"{BASE_URL}/health") as r:
                data = await r.json()
                print(f"  ✅ {BASE_URL}  {data}")
    except Exception as e:
        print(f"  ❌ {BASE_URL}  ОШИБКА: {e}")
        print("\n  Запустите проект:")
        print("    cd task && docker compose up -d --build")
        return

    # ── Тест 1: Latency ──────────────────────────────────────────

    banner("Тест 1a: Latency — 100 последовательных GET /books/1")
    lats_1a = await sequential_latency_test("/books/1", 100)
    print_latency("Карточка книги (GET /books/1)", lats_1a)

    banner("Тест 1b: Latency — 100 последовательных GET /authors/")
    lats_1b = await sequential_latency_test("/authors/", 100)
    print_latency("Список авторов (GET /authors/)", lats_1b)

    banner("Тест 1c: Latency — 100 последовательных GET /books/top-rated")
    lats_1c = await sequential_latency_test("/books/top-rated?limit=10", 100)
    print_latency("Топ по рейтингу (GET /books/top-rated)", lats_1c)

    # ── Тест 2: Нагрузка — 50 пользователей ─────────────────────

    concurrency, total = 50, 2000
    banner(f"Тест 2: Нагрузка — {concurrency} пользователей, {total} запросов")
    data2, elapsed2 = await parallel_load(concurrency, total)
    print_load(f"Параллельная нагрузка ({concurrency} users, {total} requests)", data2, elapsed2, total)

    # ── Тест 3: Тяжёлая нагрузка — 100 пользователей ────────────

    concurrency3, total3 = 100, 5000
    banner(f"Тест 3: Тяжёлая нагрузка — {concurrency3} пользователей, {total3} запросов")
    data3, elapsed3 = await parallel_load(concurrency3, total3)
    print_load(f"Тяжёлая нагрузка ({concurrency3} users, {total3} requests)", data3, elapsed3, total3)

    print()
    print("═" * 68)
    print("  Бенчмарк завершён!")
    print("═" * 68)
    print()


if __name__ == "__main__":
    asyncio.run(main())
