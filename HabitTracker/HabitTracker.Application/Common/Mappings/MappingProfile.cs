using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; // Sử dụng cho reflection để quét assembly và invoke methods
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Common.Mappings
{
    /// <summary>
    /// Profile chính của AutoMapper để tự động đăng ký tất cả các mappings
    /// từ các lớp implement IMapFrom&lt;T&gt; trong assembly hiện tại.
    /// Điều này giúp tránh việc phải viết thủ công CreateMap cho từng ViewModel/DTO.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Constructor: Tự động áp dụng tất cả mappings từ assembly hiện tại
        /// khi AutoMapper được khởi tạo qua DI (ví dụ: AddAutoMapper).
        /// </summary>
        public MappingProfile()
        {
            // Quét và đăng ký mappings từ assembly chứa lớp này (thường là Application assembly)
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Phương thức private để quét assembly và đăng ký mappings tự động.
        /// Sử dụng reflection để tìm các loại (types) implement IMapFrom&lt;T&gt;
        /// và gọi method Mapping trên chúng để tạo CreateMap.
        /// </summary>
        /// <param name="assembly">Assembly cần quét (thường là assembly hiện tại).</param>
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            // Định nghĩa interface generic IMapFrom<T> để kiểm tra
            var mapFromType = typeof(IMapFrom<>);

            // Tên method cần gọi trên các lớp implement IMapFrom (từ default implementation của interface)
            var mappingMethodName = nameof(IMapFrom<object>.Mapping);

            // Lambda helper: Kiểm tra xem một interface có phải là IMapFrom<T> không
            // (so sánh generic type definition để tránh so sánh instance cụ thể)
            bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

            // Lấy tất cả các loại public/exported từ assembly,
            // lọc những loại có ít nhất một interface là IMapFrom<T>
            var types = assembly.GetExportedTypes()
                                .Where(t => t.GetInterfaces().Any(HasInterface))
                                .ToList();

            // Mảng kiểu tham số cho method invoke (method Mapping nhận Profile)
            var argumentTypes = new Type[] { typeof(Profile) };

            // Duyệt qua từng loại (ví dụ: HabitViewModel) implement IMapFrom
            foreach (var type in types)
            {
                // Tạo instance tạm thời của type để gọi method (không cần DI đầy đủ)
                var instance = Activator.CreateInstance(type);

                // Tìm method 'Mapping' trực tiếp trên type (nếu override)
                var methodInfo = type.GetMethod(mappingMethodName);

                // Nếu method tồn tại trên type, gọi nó với 'this' (Profile hiện tại)
                if (methodInfo != null)
                {
                    methodInfo.Invoke(instance, new object[] { this });
                }
                else
                {
                    // Nếu không có method override, tìm trên các interfaces IMapFrom<T>
                    var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                    // Nếu có interfaces, duyệt và gọi method trên từng interface
                    if (interfaces.Count > 0)
                    {
                        foreach (var @interface in interfaces)
                        {
                            // Lấy method 'Mapping' từ interface với binding flags phù hợp
                            var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                            // Gọi method nếu tồn tại, truyền 'this' làm tham số
                            interfaceMethodInfo?.Invoke(instance, new object[] { this });
                        }
                    }
                }
            }
        }
    }
}