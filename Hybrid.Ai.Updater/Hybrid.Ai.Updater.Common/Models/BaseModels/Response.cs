using System.Collections.Generic;

namespace Hybrid.ai.Geoposition.Common.Models.BaseModels
{
    public class Response
    {
        public class CustomErrors
	{
		public ResponseCodes ResponseCode { get; set; }
		public string ResultMessage { get; set; }
		public int? BatchId { get; set; }
	}

	public class AppResponse
	{
		public class Response
		{
			public string Code => ResultCode.ToString();

			public ResponseCodes ResultCode { get; set; } = ResponseCodes.SUCCESS;

			public List<CustomErrors> Errors { get; set; }
		}

		public class Response<T> : Response
		{
			public Response()
			{
				
			}
			
	        public Response( T data) => Data = data;
	        public T Data { get; set; }
        }

		public class ErrorResponse : Response
		{
			public ErrorResponse(string errorMessage, ResponseCodes responseCode = ResponseCodes.TECHNICAL_ERROR)
			{
				ResultCode = ResponseCodes.FAILURE;

				if (Errors == null)
					Errors = new List<CustomErrors>();

				Errors.Add(new CustomErrors
				{
					ResponseCode = responseCode,
					ResultMessage = errorMessage
				});
			}

			public ErrorResponse(IEnumerable<CustomErrors> errors)
			{
				ResultCode = ResponseCodes.FAILURE;
				if (Errors == null)
					Errors = new List<CustomErrors>();

				Errors.AddRange(errors);
			}
		}

		public class ErrorResponse<T> : Response<T>
		{
			public ErrorResponse(string errorMessage, ResponseCodes responseCode = ResponseCodes.TECHNICAL_ERROR)
			{
				ResultCode = ResponseCodes.FAILURE;
				if (Errors == null)
					Errors = new List<CustomErrors>();

				Errors.Add(new CustomErrors
				{
					ResponseCode = responseCode,
					ResultMessage = errorMessage
				});
			}

			public ErrorResponse(IEnumerable<CustomErrors> errors)
			{
				ResultCode = ResponseCodes.FAILURE;
				if (Errors == null)
					Errors = new List<CustomErrors>();

				Errors.AddRange(errors);
			}
		}

		public class BatchedErrorResponse<T> : ErrorResponse
		{
			public List<T> Data { get; set; }

			public BatchedErrorResponse(ResponseCodes responseCode, string errorMessage, IEnumerable<T> data) : base(errorMessage, responseCode)
			{
				if (Data == null)
					Data = new List<T>();

				Data.AddRange(data);
			}

			public BatchedErrorResponse(IEnumerable<CustomErrors> errors, IEnumerable<T> data) : base(errors)
			{
				if (Data == null)
                    Data = new List<T>();

				Data.AddRange(data);
			}
		}	
	}
    }
}