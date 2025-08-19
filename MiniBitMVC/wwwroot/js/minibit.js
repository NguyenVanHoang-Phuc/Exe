// MiniBit JavaScript Functions
class MiniBitApp {
    constructor() {
        this.expenses = window.expensesData || []
        this.savingsGoal = window.savingsGoalData || { monthlyTarget: 0, dailyLimit: 0 }
        this.isPremium = window.isPremium || false
        this.notifications = []

        this.init()
    }

    init() {
        this.updateDashboard()
        this.updateCharacterMessage()
        this.checkNotifications()
        this.bindEvents()

        if (this.isPremium) {
            this.showPremiumFeatures()
        }
    }

    bindEvents() {
        // Premium upgrade
        document.getElementById("premiumBtn").addEventListener("click", () => {
            this.upgradeToPremium()
        })

        // Savings goal form
        document.getElementById("savingsGoalForm").addEventListener("submit", (e) => {
            e.preventDefault()
            this.saveSavingsGoal()
        })

        // Notification bell
        document.getElementById("notificationBell").addEventListener("click", () => {
            this.showNotifications()
        })
    }

    updateDashboard() {
        const today = new Date().toISOString().split("T")[0]
        const currentMonth = new Date().getMonth()
        const currentYear = new Date().getFullYear()

        // Calculate totals
        const todayExpenses = this.expenses.filter((e) => e.date.startsWith(today))
        const monthlyExpenses = this.expenses.filter((e) => {
            const expenseDate = new Date(e.date)
            return expenseDate.getMonth() === currentMonth && expenseDate.getFullYear() === currentYear
        })

        const todayTotal = todayExpenses.reduce((sum, e) => sum + e.amount, 0)
        const monthlyTotal = monthlyExpenses.reduce((sum, e) => sum + e.amount, 0)
        const monthlySavings = this.savingsGoal.monthlyTarget - monthlyTotal

        // Update UI
        document.getElementById("todayTotal").textContent = this.formatCurrency(todayTotal)
        document.getElementById("monthlyTotal").textContent = this.formatCurrency(monthlyTotal)
        document.getElementById("savingsAmount").textContent = this.formatCurrency(monthlySavings)

        // Update progress bars
        if (this.savingsGoal.dailyLimit > 0) {
            const todayProgress = Math.min((todayTotal / this.savingsGoal.dailyLimit) * 100, 100)
            document.getElementById("todayProgress").style.width = todayProgress + "%"
        }

        if (this.savingsGoal.monthlyTarget > 0) {
            const monthlyProgress = Math.min((monthlyTotal / this.savingsGoal.monthlyTarget) * 100, 100)
            document.getElementById("monthlyProgress").style.width = monthlyProgress + "%"
        }

        // Update savings status
        const savingsStatus = document.getElementById("savingsStatus")
        const savingsAmount = document.getElementById("savingsAmount")

        if (monthlySavings >= 0) {
            savingsStatus.textContent = "Đang tiết kiệm tốt!"
            savingsAmount.className = "text-success mb-2"
        } else {
            savingsStatus.textContent = "Cần cải thiện!"
            savingsAmount.className = "text-danger mb-2"
        }
    }

    updateCharacterMessage() {
        const today = new Date().toISOString().split("T")[0]
        const todayExpenses = this.expenses.filter((e) => e.date.startsWith(today))
        const todayTotal = todayExpenses.reduce((sum, e) => sum + e.amount, 0)

        let message = ""

        if (this.notifications.length > 0) {
            message = this.notifications[0]
        } else if (todayTotal === 0) {
            message = "Chào bạn! Hôm nay bạn chưa ghi nhận chi tiêu nào nhé! 💚"
        } else if (this.savingsGoal.dailyLimit > 0 && todayTotal > this.savingsGoal.dailyLimit) {
            message = `Ôi không! Bạn đã vượt ngân sách hôm nay rồi! Hãy cẩn thận hơn nhé! 😰 (${this.formatCurrency(todayTotal)})`
        } else if (this.savingsGoal.dailyLimit > 0 && todayTotal > this.savingsGoal.dailyLimit * 0.8) {
            message = `Cẩn thận! Bạn đã chi gần hết ngân sách hôm nay rồi! 😟 (${this.formatCurrency(todayTotal)})`
        } else {
            message = `Hôm nay bạn đã chi ${this.formatCurrency(todayTotal)}. Hãy tiếp tục theo dõi nhé! 🌱`
        }

        document.getElementById("characterMessage").textContent = message
    }

    checkNotifications() {
        this.notifications = []
        const today = new Date().toISOString().split("T")[0]
        const todayExpenses = this.expenses.filter((e) => e.date.startsWith(today))
        const todayTotal = todayExpenses.reduce((sum, e) => sum + e.amount, 0)

        // Daily limit notification
        if (this.savingsGoal.dailyLimit > 0 && todayTotal > this.savingsGoal.dailyLimit) {
            this.notifications.push(
                `⚠️ Bạn đã vượt mức chi tiêu hàng ngày! (${this.formatCurrency(todayTotal)} / ${this.formatCurrency(this.savingsGoal.dailyLimit)})`,
            )
        }

        // Monthly warning
        const currentMonth = new Date().getMonth()
        const currentYear = new Date().getFullYear()
        const monthlyExpenses = this.expenses.filter((e) => {
            const expenseDate = new Date(e.date)
            return expenseDate.getMonth() === currentMonth && expenseDate.getFullYear() === currentYear
        })
        const monthlyTotal = monthlyExpenses.reduce((sum, e) => sum + e.amount, 0)

        if (this.savingsGoal.monthlyTarget > 0 && monthlyTotal > this.savingsGoal.monthlyTarget * 0.8) {
            this.notifications.push("🚨 Chi tiêu tháng này đã đạt 80% mục tiêu tiết kiệm!")
        }

        // Yesterday notification (simulated)
        const yesterday = new Date()
        yesterday.setDate(yesterday.getDate() - 1)
        const yesterdayStr = yesterday.toISOString().split("T")[0]
        const yesterdayExpenses = this.expenses.filter((e) => e.date.startsWith(yesterdayStr))
        const yesterdayTotal = yesterdayExpenses.reduce((sum, e) => sum + e.amount, 0)

        if (yesterdayTotal > 0) {
            this.notifications.push(`📊 Hôm qua bạn đã chi tiêu ${this.formatCurrency(yesterdayTotal)}`)
        }

        // Update notification badge
        document.getElementById("notificationBadge").textContent = this.notifications.length
    }

    showNotifications() {
        if (this.notifications.length === 0) {
            alert("Không có thông báo mới!")
            return
        }

        const notificationText = this.notifications.join("\n\n")
        alert(notificationText)
    }

    addExpense() {
        const amount = Number.parseFloat(document.getElementById("expenseAmount").value)
        const category = document.getElementById("expenseCategory").value
        const description = document.getElementById("expenseDescription").value
        const type = document.querySelector('input[name="expenseType"]:checked').value

        if (!amount || !category) {
            alert("Vui lòng điền đầy đủ thông tin!")
            return
        }

        const expense = {
            id: Date.now(),
            amount: amount,
            category: category,
            description: description,
            type: type,
            date: new Date().toISOString(),
        }

        this.expenses.push(expense)
        this.updateDashboard()
        this.updateCharacterMessage()
        this.checkNotifications()
        this.addExpenseToUI(expense)

        // Close modal and reset form
        const modal = bootstrap.Modal.getInstance(document.getElementById("addExpenseModal"))
        modal.hide()
        document.getElementById("addExpenseForm").reset()

        // Show success message
        this.showToast("Chi tiêu đã được thêm thành công!", "success")
    }

    addExpenseToUI(expense) {
        const expenseHTML = `
            <div class="expense-item ${expense.type === "good" ? "bg-success" : "bg-danger"} bg-opacity-10">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h6 class="mb-1">${expense.category}</h6>
                        <p class="mb-0 text-muted small">${expense.description}</p>
                    </div>
                    <strong class="${expense.type === "good" ? "text-success" : "text-danger"}">${this.formatCurrency(expense.amount)}</strong>
                </div>
            </div>
        `

        const container = expense.type === "good" ? "goodExpenses" : "badExpenses"
        document.getElementById(container).insertAdjacentHTML("afterbegin", expenseHTML)

        // Update recent expenses
        this.updateRecentExpenses()
    }

    updateRecentExpenses() {
        const recentExpenses = this.expenses.sort((a, b) => new Date(b.date) - new Date(a.date)).slice(0, 5)

        const container = document.getElementById("recentExpenses")

        if (recentExpenses.length === 0) {
            container.innerHTML = `
                <div class="text-center py-5">
                    <i class="fas fa-receipt fa-3x text-muted mb-3"></i>
                    <p class="text-muted">Chưa có chi tiêu nào được ghi nhận</p>
                </div>
            `
            return
        }

        container.innerHTML = recentExpenses
            .map(
                (expense) => `
            <div class="expense-item">
                <div class="d-flex align-items-center">
                    <div class="expense-icon me-3">
                        <i class="fas fa-${expense.type === "good" ? "check" : "times"}-circle text-${expense.type === "good" ? "success" : "danger"}"></i>
                    </div>
                    <div class="flex-grow-1">
                        <h6 class="mb-1">${expense.category}</h6>
                        <p class="mb-0 text-muted small">${expense.description}</p>
                    </div>
                    <div class="text-end">
                        <strong>${this.formatCurrency(expense.amount)}</strong>
                        <br>
                        <small class="text-muted">${new Date(expense.date).toLocaleDateString("vi-VN")}</small>
                    </div>
                </div>
            </div>
        `,
            )
            .join("")
    }

    saveSavingsGoal() {
        const monthlyTarget = Number.parseFloat(document.getElementById("monthlyTarget").value) || 0
        const dailyLimit = Number.parseFloat(document.getElementById("dailyLimit").value) || 0

        this.savingsGoal = { monthlyTarget, dailyLimit }

        this.updateDashboard()
        this.updateCharacterMessage()
        this.checkNotifications()
        this.showGoalSummary()

        this.showToast("Mục tiêu đã được lưu thành công!", "success")
    }

    showGoalSummary() {
        if (this.savingsGoal.monthlyTarget > 0) {
            document.getElementById("targetDisplay").textContent =
                this.formatCurrency(this.savingsGoal.monthlyTarget) + "/tháng"
            document.getElementById("limitDisplay").textContent = this.formatCurrency(this.savingsGoal.dailyLimit) + "/ngày"

            const currentMonth = new Date().getMonth()
            const currentYear = new Date().getFullYear()
            const monthlyExpenses = this.expenses.filter((e) => {
                const expenseDate = new Date(e.date)
                return expenseDate.getMonth() === currentMonth && expenseDate.getFullYear() === currentYear
            })
            const monthlyTotal = monthlyExpenses.reduce((sum, e) => sum + e.amount, 0)
            const progress = ((monthlyTotal / this.savingsGoal.monthlyTarget) * 100).toFixed(1)

            document.getElementById("progressDisplay").textContent = progress + "%"
            document.getElementById("goalSummary").style.display = "block"
        }
    }

    upgradeToPremium() {
        this.isPremium = true
        this.showPremiumFeatures()

        // Update premium button
        const premiumBtn = document.getElementById("premiumBtn")
        premiumBtn.innerHTML = '<i class="fas fa-crown me-2"></i>Premium'
        premiumBtn.classList.add("btn-success")
        premiumBtn.classList.remove("btn-premium")

        this.showToast("Chào mừng bạn đến với MiniBit Premium! 🎉", "success")
    }

    showPremiumFeatures() {
        document.getElementById("premiumPromo").style.display = "none"
        document.getElementById("premiumContent").style.display = "block"

        this.updateCategoryAnalysis()
    }

    updateCategoryAnalysis() {
        const categoryTotals = {}
        this.expenses.forEach((expense) => {
            categoryTotals[expense.category] = (categoryTotals[expense.category] || 0) + expense.amount
        })

        const analysisHTML = Object.entries(categoryTotals)
            .sort(([, a], [, b]) => b - a)
            .map(
                ([category, amount]) => `
                <div class="d-flex justify-content-between align-items-center py-2 border-bottom">
                    <span>${category}</span>
                    <strong>${this.formatCurrency(amount)}</strong>
                </div>
            `,
            )
            .join("")

        document.getElementById("categoryAnalysis").innerHTML =
            analysisHTML || '<p class="text-muted">Chưa có dữ liệu phân tích</p>'
    }

    formatCurrency(amount) {
        return new Intl.NumberFormat("vi-VN").format(amount) + "đ"
    }

    showToast(message, type = "info") {
        // Create toast element
        const toast = document.createElement("div")
        toast.className = `alert alert-${type} alert-dismissible fade show position-fixed`
        toast.style.cssText = "top: 20px; right: 20px; z-index: 9999; min-width: 300px;"
        toast.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `

        document.body.appendChild(toast)

        // Auto remove after 3 seconds
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast)
            }
        }, 3000)
    }
}

// Global functions for HTML onclick events
function addExpense() {
    window.miniBitApp.addExpense()
}

function upgradeToPremium() {
    window.miniBitApp.upgradeToPremium()
}

// Initialize app when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
    window.miniBitApp = new MiniBitApp()
})
