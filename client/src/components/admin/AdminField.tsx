import type { InputHTMLAttributes, TextareaHTMLAttributes } from 'react'

interface FieldProps {
  label: string
  error?: string
}

type InputFieldProps = FieldProps & InputHTMLAttributes<HTMLInputElement> & { as?: 'input' }
type TextareaFieldProps = FieldProps & TextareaHTMLAttributes<HTMLTextAreaElement> & { as: 'textarea' }

type AdminFieldProps = InputFieldProps | TextareaFieldProps

const baseClass =
  'w-full rounded-xl border border-white/10 bg-white/5 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none transition focus:border-cyan-500/60 focus:bg-white/8 disabled:opacity-50'

export default function AdminField(props: AdminFieldProps) {
  const { label, error, as, ...rest } = props as AdminFieldProps & { as?: string }
  return (
    <div>
      <label className="mb-1.5 block text-xs font-medium text-slate-400">{label}</label>
      {as === 'textarea' ? (
        <textarea
          rows={3}
          className={`${baseClass} resize-none`}
          {...(rest as TextareaHTMLAttributes<HTMLTextAreaElement>)}
        />
      ) : (
        <input className={baseClass} {...(rest as InputHTMLAttributes<HTMLInputElement>)} />
      )}
      {error && <p className="mt-1 text-xs text-red-400">{error}</p>}
    </div>
  )
}
