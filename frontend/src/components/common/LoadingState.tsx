export default function LoadingState() {
  return (
    <div className="space-y-4" aria-label="Loading recommendations">
      {[0, 1, 2].map((item) => (
        <div
          key={item}
          className="overflow-hidden rounded-[28px] border border-white/8 bg-slate-900/55 p-5"
        >
          <div className="animate-pulse">
            <div className="flex items-center justify-between gap-4">
              <div className="flex items-center gap-3">
                <div className="size-12 rounded-2xl bg-white/[0.07]" />
                <div>
                  <div className="h-5 w-32 rounded bg-white/[0.08]" />
                  <div className="mt-2 h-3 w-48 rounded bg-white/[0.05]" />
                </div>
              </div>
              <div className="size-16 rounded-full bg-white/[0.07]" />
            </div>
            <div className="mt-6 grid grid-cols-3 gap-2 sm:grid-cols-6">
              {[0, 1, 2, 3, 4, 5].map((bar) => (
                <div key={bar} className="h-14 rounded-xl bg-white/[0.05]" />
              ))}
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}
