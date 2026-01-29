# TourApp Demo Guide

## Overview

This document describes the demo flow for TourApp, covering all major features across Tourist, Guide, and Admin roles.

---

## Test Accounts

| Role    | Username     | Password    |
|---------|-------------|-------------|
| Tourist | tourist1    | password123 |
| Tourist | tourist2    | password123 |
| Guide   | guide1      | password123 |
| Guide   | guide2      | password123 |
| Admin   | admin       | password123 |

---

## Quick URLs

| Feature                  | URL                                  | Role Required |
|-------------------------|--------------------------------------|---------------|
| Home                    | `/`                                  | Public        |
| Browse Tours            | `/tours`                             | Public        |
| Tour Details            | `/tours/:id`                         | Public        |
| Login                   | `/login`                             | Public        |
| **Tourist**             |                                      |               |
| My Area                 | `/tourist-area`                      | Tourist       |
| Cart                    | `/cart`                              | Tourist       |
| My Purchases            | `/tourist/my-purchases`              | Tourist       |
| Rate Tour               | `/tourist/ratings/new?tourId=:id`    | Tourist       |
| Report Problem          | `/tourist/problems/new?tourId=:id`   | Tourist       |
| **Guide**               |                                      |               |
| My Area                 | `/guide-area`                        | Guide         |
| My Tours                | `/guide/tours`                       | Guide         |
| Create Tour             | `/guide/tours/create`                | Guide         |
| Manage Tour             | `/guide/tours/:id`                   | Guide         |
| Replacement Board       | `/guide/replacements`                | Guide         |
| My Problems             | `/guide/problems`                    | Guide         |
| **Admin**               |                                      |               |
| My Area                 | `/admin-area`                        | Admin         |
| Blocked Users           | `/admin/blocked-users`               | Admin         |
| Problems Review         | `/admin/problems-review`             | Admin         |

---

## Demo Flow

### 1. Tourist: Browse → Cart → Purchase

**Duration:** ~3 minutes

1. **Login as Tourist**
   - Navigate to `/login`
   - Username: `tourist1`, Password: `password123`

2. **Browse Tours**
   - Navigate to `/tours`
   - View available published tours
   - Note: Tours with "Replacement Requested" badge cannot be added to cart

3. **View Tour Details**
   - Click on a tour card to view details
   - See key points, ratings, and map

4. **Add to Cart**
   - Click "Add to Cart" button
   - Navigate to `/cart`

5. **Complete Purchase**
   - Review cart items
   - Toggle "Use bonus points" if available
   - Click "Confirm Purchase"
   - See success message with final price

---

### 2. Guide: Request Replacement → Another Guide Takes Over

**Duration:** ~4 minutes

1. **Login as Guide 1**
   - Navigate to `/login`
   - Username: `guide1`, Password: `password123`

2. **Create a Tour (if needed)**
   - Navigate to `/guide/tours/create`
   - Fill in tour details
   - Add at least 2 key points
   - Publish the tour

3. **Request Replacement**
   - Navigate to `/guide/tours`
   - Click on a published tour
   - Click "Request Replacement" button
   - See tour badge change to "Replacement Requested"

4. **Logout and Login as Guide 2**
   - Username: `guide2`, Password: `password123`

5. **Take Over Tour**
   - Navigate to `/guide/replacements`
   - See the replacement board with available tours
   - Click "Take Over" on the tour
   - See success message
   - Tour now appears in Guide 2's tours list

---

### 3. Tourist: Rate Tour + Report Problem

**Duration:** ~3 minutes

1. **Login as Tourist**
   - Username: `tourist1`, Password: `password123`

2. **View Purchased Tour**
   - Navigate to `/tourist/my-purchases`
   - Click on a purchased tour

3. **Rate the Tour**
   - Click "⭐ Rate This Tour"
   - Select a score (1-5 stars)
   - Add a comment (required if score ≤ 2)
   - Submit rating
   - See rating appear on tour details

4. **Report a Problem**
   - On tour details, click "🚨 Report a Problem"
   - Fill in title and description
   - Submit problem
   - Redirected to problem details page

---

### 4. Guide/Admin: Change Problem Status → Event Log

**Duration:** ~3 minutes

1. **Login as Guide**
   - Username: `guide1`, Password: `password123`

2. **View Reported Problems**
   - Navigate to `/guide/problems`
   - See list of problems for your tours

3. **Handle Problem**
   - Click on a pending problem
   - Choose an action:
     - "Resolve" - Mark as resolved
     - "Send to Review" - Escalate to admin

4. **Login as Admin**
   - Username: `admin`, Password: `password123`

5. **Review Escalated Problems**
   - Navigate to `/admin/problems-review`
   - See problems in "In Review" status
   - Click on a problem

6. **Admin Actions**
   - View problem details and event log
   - Choose an action:
     - "Reject" - Reject the problem
     - "Reopen" - Send back to Guide for resolution

7. **View Event Log**
   - On problem details page, see status change history
   - Each event shows: old status → new status, timestamp, user role

---

## Edge Cases to Demonstrate

### Cart Restrictions
- Canceled tours show "Canceled" label and cannot be added to cart
- Tours with replacement requested show "Awaiting Replacement" and cannot be added

### Rating Validation
- Score ≤ 2 requires a comment (validation message shown)
- Only purchased tours can be rated (backend validation)

### Replacement Conflicts
- Guide cannot take over a tour if they already have a tour on the same date
- Error message: "Guide already has a tour on this date"

### Bonus Points Purchase
- When total = 0 after applying bonus, purchase still succeeds
- Message shows "Used X bonus points"

---

## Smoke Test Checklist

1. [ ] Login as Tourist and browse tours
2. [ ] Add tour to cart and complete purchase
3. [ ] Login as Guide and create/publish tour with 2+ keypoints
4. [ ] Request replacement for published tour
5. [ ] Login as different Guide and take over tour
6. [ ] Login as Tourist, rate tour, and report problem
7. [ ] Login as Guide and resolve/escalate problem
8. [ ] Login as Admin and review escalated problem
9. [ ] Verify 403 redirect works (access admin page as tourist)
10. [ ] Verify 404 page for invalid routes

---

## Known Limitations

- No real payment processing (demo mode)
- Email notifications not implemented
- Image upload uses URL input only
- No real-time updates (manual refresh required)
